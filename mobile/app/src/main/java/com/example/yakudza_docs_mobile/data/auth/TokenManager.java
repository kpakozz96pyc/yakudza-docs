package com.example.yakudza_docs_mobile.data.auth;

import android.content.Context;
import android.content.SharedPreferences;

public class TokenManager {
    private static final String PREF_NAME = "yakudza_auth";
    private static final String KEY_TOKEN = "auth_token";

    private final SharedPreferences sharedPreferences;

    public TokenManager(Context context) {
        this.sharedPreferences = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
    }

    public void saveToken(String token) {
        sharedPreferences.edit()
                .putString(KEY_TOKEN, token)
                .apply();
    }

    public String getToken() {
        return sharedPreferences.getString(KEY_TOKEN, null);
    }

    public boolean hasToken() {
        return getToken() != null;
    }

    public void clearToken() {
        sharedPreferences.edit()
                .remove(KEY_TOKEN)
                .apply();
    }
}
