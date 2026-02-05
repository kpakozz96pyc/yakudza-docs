package com.example.yakudza_docs_mobile.ui.detail;

import android.app.Application;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.yakudza_docs_mobile.data.model.DishDetail;
import com.example.yakudza_docs_mobile.data.repository.DishRepository;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class DishDetailViewModel extends AndroidViewModel {
    private final DishRepository dishRepository;
    private final MutableLiveData<DishDetail> dishDetail = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loading = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();

    public DishDetailViewModel(Application application) {
        super(application);
        this.dishRepository = new DishRepository(application);
    }

    public void loadDishDetail(int dishId) {
        loading.setValue(true);
        error.setValue(null);

        dishRepository.getDishDetail(dishId, new Callback<DishDetail>() {
            @Override
            public void onResponse(Call<DishDetail> call, Response<DishDetail> response) {
                loading.setValue(false);
                if (response.isSuccessful() && response.body() != null) {
                    dishDetail.setValue(response.body());
                } else {
                    error.setValue("Failed to load dish details");
                }
            }

            @Override
            public void onFailure(Call<DishDetail> call, Throwable t) {
                loading.setValue(false);
                error.setValue("Network error: " + t.getMessage());
            }
        });
    }

    public LiveData<DishDetail> getDishDetail() {
        return dishDetail;
    }

    public LiveData<Boolean> getLoading() {
        return loading;
    }

    public LiveData<String> getError() {
        return error;
    }
}
