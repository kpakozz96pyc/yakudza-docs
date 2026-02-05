package com.example.yakudza_docs_mobile.data.api;

import com.example.yakudza_docs_mobile.data.auth.TokenManager;

import java.io.IOException;

import okhttp3.Interceptor;
import okhttp3.Request;
import okhttp3.Response;

public class AuthInterceptor implements Interceptor {
    private final TokenManager tokenManager;

    public AuthInterceptor(TokenManager tokenManager) {
        this.tokenManager = tokenManager;
    }

    @Override
    public Response intercept(Chain chain) throws IOException {
        Request originalRequest = chain.request();

        String token = tokenManager.getToken();
        if (token == null) {
            return chain.proceed(originalRequest);
        }

        Request authenticatedRequest = originalRequest.newBuilder()
                .header("Authorization", "Bearer " + token)
                .build();

        return chain.proceed(authenticatedRequest);
    }
}
