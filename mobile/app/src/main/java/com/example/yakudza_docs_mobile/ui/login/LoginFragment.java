package com.example.yakudza_docs_mobile.ui.login;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;
import androidx.navigation.Navigation;

import com.example.yakudza_docs_mobile.R;
import com.example.yakudza_docs_mobile.databinding.FragmentLoginBinding;

public class LoginFragment extends Fragment {
    private FragmentLoginBinding binding;
    private LoginViewModel viewModel;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        binding = FragmentLoginBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewModel = new ViewModelProvider(this).get(LoginViewModel.class);

        binding.loginButton.setOnClickListener(v -> {
            String username = binding.usernameInput.getText().toString().trim();
            String password = binding.passwordInput.getText().toString().trim();

            if (username.isEmpty()) {
                binding.usernameLayout.setError(getString(R.string.error_username_required));
                return;
            }

            if (password.isEmpty()) {
                binding.passwordLayout.setError(getString(R.string.error_password_required));
                return;
            }

            binding.usernameLayout.setError(null);
            binding.passwordLayout.setError(null);

            viewModel.login(username, password);
        });

        viewModel.getLoading().observe(getViewLifecycleOwner(), loading -> {
            binding.progressBar.setVisibility(loading ? View.VISIBLE : View.GONE);
            binding.loginButton.setEnabled(!loading);
            binding.usernameInput.setEnabled(!loading);
            binding.passwordInput.setEnabled(!loading);
        });

        viewModel.getError().observe(getViewLifecycleOwner(), error -> {
            if (error != null) {
                Toast.makeText(requireContext(), error, Toast.LENGTH_LONG).show();
            }
        });

        viewModel.getLoginSuccess().observe(getViewLifecycleOwner(), success -> {
            if (success != null && success) {
                Navigation.findNavController(view)
                        .navigate(R.id.action_loginFragment_to_dishFeedFragment);
            }
        });
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
