package com.example.yakudza_docs_mobile.ui.login;

import android.app.Application;

import androidx.lifecycle.AndroidViewModel;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;

import com.example.yakudza_docs_mobile.data.model.LoginResponse;
import com.example.yakudza_docs_mobile.data.repository.AuthRepository;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginViewModel extends AndroidViewModel {
    private final AuthRepository authRepository;
    private final MutableLiveData<Boolean> loginSuccess = new MutableLiveData<>();
    private final MutableLiveData<Boolean> loading = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();

    public LoginViewModel(Application application) {
        super(application);
        this.authRepository = new AuthRepository(application);
    }

    public void login(String username, String password) {
        loading.setValue(true);
        error.setValue(null);

        authRepository.login(username, password, new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                loading.setValue(false);
                if (response.isSuccessful() && response.body() != null) {
                    LoginResponse loginResponse = response.body();
                    authRepository.saveToken(loginResponse.getToken());
                    loginSuccess.setValue(true);
                } else {
                    error.setValue("Invalid credentials");
                    loginSuccess.setValue(false);
                }
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                loading.setValue(false);
                error.setValue("Network error: " + t.getMessage());
                loginSuccess.setValue(false);
            }
        });
    }

    public LiveData<Boolean> getLoginSuccess() {
        return loginSuccess;
    }

    public LiveData<Boolean> getLoading() {
        return loading;
    }

    public LiveData<String> getError() {
        return error;
    }
}
