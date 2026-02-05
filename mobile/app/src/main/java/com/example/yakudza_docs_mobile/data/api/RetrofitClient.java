package com.example.yakudza_docs_mobile.data.api;

import com.example.yakudza_docs_mobile.data.auth.TokenManager;

import okhttp3.OkHttpClient;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class RetrofitClient {
    private static final String BASE_URL = "http://kpakozz96pyc.xyz:8447/api/";
    private static RetrofitClient instance;
    private final ApiService apiService;

    private RetrofitClient(TokenManager tokenManager) {
        HttpLoggingInterceptor loggingInterceptor = new HttpLoggingInterceptor();
        loggingInterceptor.setLevel(HttpLoggingInterceptor.Level.BODY);

        OkHttpClient client = new OkHttpClient.Builder()
                .addInterceptor(new AuthInterceptor(tokenManager))
                .addInterceptor(loggingInterceptor)
                .build();

        Retrofit retrofit = new Retrofit.Builder()
                .baseUrl(BASE_URL)
                .client(client)
                .addConverterFactory(GsonConverterFactory.create())
                .build();

        apiService = retrofit.create(ApiService.class);
    }

    public static synchronized RetrofitClient getInstance(TokenManager tokenManager) {
        if (instance == null) {
            instance = new RetrofitClient(tokenManager);
        }
        return instance;
    }

    public ApiService getApiService() {
        return apiService;
    }
}
