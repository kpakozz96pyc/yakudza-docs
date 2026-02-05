package com.example.yakudza_docs_mobile.data.api;

import com.example.yakudza_docs_mobile.data.model.DishDetail;
import com.example.yakudza_docs_mobile.data.model.DishesResponse;
import com.example.yakudza_docs_mobile.data.model.LoginRequest;
import com.example.yakudza_docs_mobile.data.model.LoginResponse;

import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface ApiService {
    @POST("auth/login")
    Call<LoginResponse> login(@Body LoginRequest request);

    @GET("dishes")
    Call<DishesResponse> getDishes(
            @Query("page") int page,
            @Query("pageSize") int pageSize,
            @Query("search") String search
    );

    @GET("dishes/{id}")
    Call<DishDetail> getDishDetail(@Path("id") int dishId);

    @GET("dishes/{id}/image")
    Call<ResponseBody> getDishImage(@Path("id") int dishId);
}
