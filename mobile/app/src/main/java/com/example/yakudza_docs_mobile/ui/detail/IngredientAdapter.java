package com.example.yakudza_docs_mobile.ui.detail;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.example.yakudza_docs_mobile.R;
import com.example.yakudza_docs_mobile.data.model.Ingredient;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class IngredientAdapter extends RecyclerView.Adapter<IngredientAdapter.IngredientViewHolder> {
    private List<Ingredient> ingredients = new ArrayList<>();

    public void setIngredients(List<Ingredient> ingredients) {
        this.ingredients = ingredients;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public IngredientViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_ingredient, parent, false);
        return new IngredientViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull IngredientViewHolder holder, int position) {
        Ingredient ingredient = ingredients.get(position);
        holder.bind(ingredient);
    }

    @Override
    public int getItemCount() {
        return ingredients.size();
    }

    static class IngredientViewHolder extends RecyclerView.ViewHolder {
        private final TextView ingredientName;
        private final TextView ingredientWeight;

        public IngredientViewHolder(@NonNull View itemView) {
            super(itemView);
            ingredientName = itemView.findViewById(R.id.ingredientName);
            ingredientWeight = itemView.findViewById(R.id.ingredientWeight);
        }

        public void bind(Ingredient ingredient) {
            ingredientName.setText(ingredient.getName());
            ingredientWeight.setText(String.format(Locale.getDefault(), "%.1f g", ingredient.getWeightGrams()));
        }
    }
}
