package com.example.yakudza_docs_mobile.data.repository;

import android.content.Context;

import com.example.yakudza_docs_mobile.data.api.ApiService;
import com.example.yakudza_docs_mobile.data.api.RetrofitClient;
import com.example.yakudza_docs_mobile.data.auth.TokenManager;
import com.example.yakudza_docs_mobile.data.model.LoginRequest;
import com.example.yakudza_docs_mobile.data.model.LoginResponse;

import retrofit2.Call;
import retrofit2.Callback;

public class AuthRepository {
    private final ApiService apiService;
    private final TokenManager tokenManager;

    public AuthRepository(Context context) {
        this.tokenManager = new TokenManager(context);
        this.apiService = RetrofitClient.getInstance(tokenManager).getApiService();
    }

    public void login(String username, String password, Callback<LoginResponse> callback) {
        LoginRequest request = new LoginRequest(username, password);
        Call<LoginResponse> call = apiService.login(request);
        call.enqueue(callback);
    }

    public void saveToken(String token) {
        tokenManager.saveToken(token);
    }

    public boolean isLoggedIn() {
        return tokenManager.hasToken();
    }

    public void logout() {
        tokenManager.clearToken();
    }
}
