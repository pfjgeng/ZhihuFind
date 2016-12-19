using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Telephony;
using Android.Text;

namespace ZhihuFind.Droid.UI.Shareds
{
    public class BaseShared
    {
        private volatile static string SecretKey;

        private ISharedPreferences sp;

        private BaseShared(Context context, string name)
        {
            sp = context.GetSharedPreferences(name, FileCreationMode.Private);
        }
        public static BaseShared With(Context context, string name)
        {
            if (TextUtils.IsEmpty(SecretKey))
            {
                lock (typeof(BaseShared))
                {
                    if (TextUtils.IsEmpty(SecretKey))
                    {
                        TelephonyManager tm = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
                        SecretKey = tm.DeviceId;
                    }
                    if (TextUtils.IsEmpty(SecretKey))
                    {
                        SecretKey = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
                    }
                    if (TextUtils.IsEmpty(SecretKey))
                    {
                        SecretKey = Build.Serial;
                    }
                    if (TextUtils.IsEmpty(SecretKey))
                    {
                        SecretKey = context.PackageName;
                    }
                }
            }
            return new BaseShared(context, name);
        }
        public int GetInt(string key, int defValue)
        {
            return Convert.ToInt32(Get(key, defValue.ToString()));
        }
        public string GetString(string key, string defValue)
        {
            return Get(key, defValue);
        }
        public string Get(string key, string defValue)
        {
            try
            {
                string spValue = sp.GetString(key, "");
                if (TextUtils.IsEmpty(spValue))
                {
                    return defValue;
                }
                else
                {
                    return spValue;
                }
            }
            catch (Exception e)
            {
                return defValue;
            }
        }
        public void Set(string key, string value)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            try
            {
                editor.PutString(key, value);
            }
            catch (Exception e)
            {
                editor.PutString(key, "");
            }
            editor.Apply();
        }
        public void SetString(string key, string value)
        {
            Set(key, value);
        }
        public void SetInt(string key, int value)
        {
            Set(key, value.ToString());
        }
        public void Clear()
        {
            sp.Edit().Clear().Apply();
        }

    }
}