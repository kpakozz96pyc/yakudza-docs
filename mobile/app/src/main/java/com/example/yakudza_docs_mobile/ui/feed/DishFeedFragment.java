package com.example.yakudza_docs_mobile.ui.feed;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.widget.SearchView;
import androidx.core.view.MenuProvider;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Lifecycle;
import androidx.lifecycle.ViewModelProvider;
import androidx.navigation.Navigation;
import androidx.recyclerview.widget.RecyclerView;

import com.example.yakudza_docs_mobile.R;
import com.example.yakudza_docs_mobile.data.repository.AuthRepository;
import com.example.yakudza_docs_mobile.databinding.FragmentDishFeedBinding;

public class DishFeedFragment extends Fragment {
    private FragmentDishFeedBinding binding;
    private DishFeedViewModel viewModel;
    private DishAdapter adapter;
    private AuthRepository authRepository;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        binding = FragmentDishFeedBinding.inflate(inflater, container, false);
        return binding.getRoot();
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        viewModel = new ViewModelProvider(this).get(DishFeedViewModel.class);
        authRepository = new AuthRepository(requireContext());

        adapter = new DishAdapter(dish -> {
            Bundle args = new Bundle();
            args.putInt("dishId", dish.getId());
            Navigation.findNavController(view)
                    .navigate(R.id.action_dishFeedFragment_to_dishDetailFragment, args);
        });

        binding.dishRecyclerView.setAdapter(adapter);

        binding.dishRecyclerView.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
                super.onScrolled(recyclerView, dx, dy);
                if (!recyclerView.canScrollVertically(1)) {
                    viewModel.loadDishes(false);
                }
            }
        });

        binding.swipeRefresh.setOnRefreshListener(() -> viewModel.loadDishes(true));

        viewModel.getDishes().observe(getViewLifecycleOwner(), dishes -> {
            adapter.setDishes(dishes);
        });

        viewModel.getLoading().observe(getViewLifecycleOwner(), loading -> {
            binding.swipeRefresh.setRefreshing(loading);
        });

        viewModel.getError().observe(getViewLifecycleOwner(), error -> {
            if (error != null) {
                Toast.makeText(requireContext(), error, Toast.LENGTH_LONG).show();
            }
        });

        setupMenu();
        viewModel.loadDishes(true);
    }

    private void setupMenu() {
        requireActivity().addMenuProvider(new MenuProvider() {
            @Override
            public void onCreateMenu(@NonNull Menu menu, @NonNull MenuInflater menuInflater) {
                menuInflater.inflate(R.menu.menu_feed, menu);

                MenuItem searchItem = menu.findItem(R.id.action_search);
                SearchView searchView = (SearchView) searchItem.getActionView();

                searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
                    @Override
                    public boolean onQueryTextSubmit(String query) {
                        viewModel.search(query);
                        return true;
                    }

                    @Override
                    public boolean onQueryTextChange(String newText) {
                        if (newText.isEmpty()) {
                            viewModel.search(null);
                        }
                        return true;
                    }
                });
            }

            @Override
            public boolean onMenuItemSelected(@NonNull MenuItem menuItem) {
                if (menuItem.getItemId() == R.id.action_logout) {
                    authRepository.logout();
                    Navigation.findNavController(binding.getRoot())
                            .navigate(R.id.action_dishFeedFragment_to_loginFragment);
                    return true;
                }
                return false;
            }
        }, getViewLifecycleOwner(), Lifecycle.State.RESUMED);
    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        binding = null;
    }
}
