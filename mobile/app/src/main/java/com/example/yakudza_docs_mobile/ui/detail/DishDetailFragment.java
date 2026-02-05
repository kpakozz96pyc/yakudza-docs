package com.example.yakudza_docs_mobile.ui.detail;

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

import com.bumptech.glide.Glide;
import com.example.yakudza_docs_mobile.R;
import com.example.yakudza_docs_mobile.databinding.FragmentDishDetailBinding;

public class DishDetailFragment extends Fragment {
    private FragmentDishDetailBinding binding;
    private DishDetailViewModel viewModel;
    private IngredientAdapter adapter;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        binding = FragmentDishDetailBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewModel = new ViewModelProvider(this).get(DishDetailViewModel.class);
        adapter = new IngredientAdapter();

        binding.ingredientsRecyclerView.setAdapter(adapter);

        binding.toolbar.setNavigationOnClickListener(v ->
                Navigation.findNavController(view).navigateUp()
        );

        int dishId = getArguments() != null ? getArguments().getInt("dishId", -1) : -1;
        if (dishId == -1) {
            Toast.makeText(requireContext(), "Invalid dish ID", Toast.LENGTH_SHORT).show();
            Navigation.findNavController(view).navigateUp();
            return;
        }

        viewModel.getDishDetail().observe(getViewLifecycleOwner(), dishDetail -> {
            if (dishDetail != null) {
                binding.collapsingToolbar.setTitle(dishDetail.getName());
                binding.dishDescription.setText(dishDetail.getDescription());

                if (dishDetail.isHasImage()) {
                    String imageUrl = "http://kpakozz96pyc.xyz:8447/api/dishes/" + dishDetail.getId() + "/image";
                    Glide.with(requireContext())
                            .load(imageUrl)
                            .placeholder(R.drawable.ic_launcher_foreground)
                            .error(R.drawable.ic_launcher_foreground)
                            .centerCrop()
                            .into(binding.dishImage);
                } else {
                    binding.dishImage.setImageResource(R.drawable.ic_launcher_foreground);
                }

                if (dishDetail.getIngredients() != null) {
                    adapter.setIngredients(dishDetail.getIngredients());
                }
            }
        });

        viewModel.getLoading().observe(getViewLifecycleOwner(), loading -> {
            binding.progressBar.setVisibility(loading ? View.VISIBLE : View.GONE);
        });

        viewModel.getError().observe(getViewLifecycleOwner(), error -> {
            if (error != null) {
                Toast.makeText(requireContext(), error, Toast.LENGTH_LONG).show();
            }
        });

        viewModel.loadDishDetail(dishId);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
