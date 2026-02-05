package com.example.yakudza_docs_mobile.ui.feed;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.example.yakudza_docs_mobile.R;
import com.example.yakudza_docs_mobile.data.model.Dish;

import java.util.ArrayList;
import java.util.List;

public class DishAdapter extends RecyclerView.Adapter<DishAdapter.DishViewHolder> {
    private List<Dish> dishes = new ArrayList<>();
    private OnDishClickListener listener;

    public interface OnDishClickListener {
        void onDishClick(Dish dish);
    }

    public DishAdapter(OnDishClickListener listener) {
        this.listener = listener;
    }

    public void setDishes(List<Dish> dishes) {
        this.dishes = dishes;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public DishViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_dish, parent, false);
        return new DishViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull DishViewHolder holder, int position) {
        Dish dish = dishes.get(position);
        holder.bind(dish, listener);
    }

    @Override
    public int getItemCount() {
        return dishes.size();
    }

    static class DishViewHolder extends RecyclerView.ViewHolder {
        private final ImageView dishImage;
        private final TextView dishName;
        private final TextView dishDescription;

        public DishViewHolder(@NonNull View itemView) {
            super(itemView);
            dishImage = itemView.findViewById(R.id.dishImage);
            dishName = itemView.findViewById(R.id.dishName);
            dishDescription = itemView.findViewById(R.id.dishDescription);
        }

        public void bind(Dish dish, OnDishClickListener listener) {
            dishName.setText(dish.getName());
            dishDescription.setText(dish.getDescription());

            if (dish.isHasImage()) {
                String imageUrl = "http://kpakozz96pyc.xyz:8447/api/dishes/" + dish.getId() + "/image";
                Glide.with(itemView.getContext())
                        .load(imageUrl)
                        .placeholder(R.drawable.ic_launcher_foreground)
                        .error(R.drawable.ic_launcher_foreground)
                        .centerCrop()
                        .into(dishImage);
            } else {
                dishImage.setImageResource(R.drawable.ic_launcher_foreground);
            }

            itemView.setOnClickListener(v -> {
                if (listener != null) {
                    listener.onDishClick(dish);
                }
            });
        }
    }
}
