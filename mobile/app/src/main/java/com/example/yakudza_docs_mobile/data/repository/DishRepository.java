package com.example.yakudza_docs_mobile.data.repository;

import android.content.Context;

import com.example.yakudza_docs_mobile.data.api.ApiService;
import com.example.yakudza_docs_mobile.data.api.RetrofitClient;
import com.example.yakudza_docs_mobile.data.auth.TokenManager;
import com.example.yakudza_docs_mobile.data.model.DishDetail;
import com.example.yakudza_docs_mobile.data.model.DishesResponse;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.Callback;

public class DishRepository {
    private final ApiService apiService;

    public DishRepository(Context context) {
        TokenManager tokenManager = new TokenManager(context);
        this.apiService = RetrofitClient.getInstance(tokenManager).getApiService();
    }

    public void getDishes(int page, int pageSize, String search, Callback<DishesResponse> callback) {
        Call<DishesResponse> call = apiService.getDishes(page, pageSize, search);
        call.enqueue(callback);
    }

    public void getDishDetail(int dishId, Callback<DishDetail> callback) {
        Call<DishDetail> call = apiService.getDishDetail(dishId);
        call.enqueue(callback);
    }

    public void getDishImage(int dishId, Callback<ResponseBody> callback) {
        Call<ResponseBody> call = apiService.getDishImage(dishId);
        call.enqueue(callback);
    }
}
