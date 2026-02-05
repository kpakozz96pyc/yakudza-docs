package com.example.yakudza_docs_mobile.data.model;

import java.util.List;

public class DishDetail extends Dish {
    private List<Ingredient> ingredients;

    public List<Ingredient> getIngredients() {
        return ingredients;
    }

    public void setIngredients(List<Ingredient> ingredients) {
        this.ingredients = ingredients;
    }
}
