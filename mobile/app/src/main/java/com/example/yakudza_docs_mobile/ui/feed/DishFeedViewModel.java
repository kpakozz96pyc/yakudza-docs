package com.example.yakudza_docs_mobile.ui.feed;

import android.app.Application;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.yakudza_docs_mobile.data.model.Dish;
import com.example.yakudza_docs_mobile.data.model.DishesResponse;
import com.example.yakudza_docs_mobile.data.repository.DishRepository;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class DishFeedViewModel extends AndroidViewModel {
    private final DishRepository dishRepository;
    private final MutableLiveData<List<Dish>> dishes = new MutableLiveData<>(new ArrayList<>());
    private final MutableLiveData<Boolean> loading = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();

    private int currentPage = 1;
    private final int pageSize = 10;
    private String currentSearch = null;
    private boolean isLastPage = false;

    public DishFeedViewModel(Application application) {
        super(application);
        this.dishRepository = new DishRepository(application);
    }

    public void loadDishes(boolean refresh) {
        if (loading.getValue() != null && loading.getValue()) {
            return;
        }

        if (refresh) {
            currentPage = 1;
            isLastPage = false;
        } else if (isLastPage) {
            return;
        }

        loading.setValue(true);
        error.setValue(null);

        dishRepository.getDishes(currentPage, pageSize, currentSearch, new Callback<DishesResponse>() {
            @Override
            public void onResponse(Call<DishesResponse> call, Response<DishesResponse> response) {
                loading.setValue(false);
                if (response.isSuccessful() && response.body() != null) {
                    DishesResponse dishesResponse = response.body();
                    List<Dish> newDishes = dishesResponse.getItems();

                    if (refresh) {
                        dishes.setValue(newDishes);
                    } else {
                        List<Dish> currentDishes = dishes.getValue();
                        if (currentDishes != null) {
                            currentDishes.addAll(newDishes);
                            dishes.setValue(currentDishes);
                        }
                    }

                    currentPage++;
                    isLastPage = newDishes.isEmpty() ||
                                (currentPage - 1) * pageSize >= dishesResponse.getTotalCount();
                } else {
                    error.setValue("Failed to load dishes");
                }
            }

            @Override
            public void onFailure(Call<DishesResponse> call, Throwable t) {
                loading.setValue(false);
                error.setValue("Network error: " + t.getMessage());
            }
        });
    }

    public void search(String query) {
        currentSearch = query != null && query.trim().isEmpty() ? null : query;
        loadDishes(true);
    }

    public LiveData<List<Dish>> getDishes() {
        return dishes;
    }

    public LiveData<Boolean> getLoading() {
        return loading;
    }

    public LiveData<String> getError() {
        return error;
    }
}
