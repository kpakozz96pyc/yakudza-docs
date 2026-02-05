package com.example.yakudza_docs_mobile.data.model;

import java.util.List;

public class DishesResponse {
    private List<Dish> items;
    private int totalCount;
    private int page;
    private int pageSize;

    public List<Dish> getItems() {
        return items;
    }

    public void setItems(List<Dish> items) {
        this.items = items;
    }

    public int getTotalCount() {
        return totalCount;
    }

    public void setTotalCount(int totalCount) {
        this.totalCount = totalCount;
    }

    public int getPage() {
        return page;
    }

    public void setPage(int page) {
        this.page = page;
    }

    public int getPageSize() {
        return pageSize;
    }

    public void setPageSize(int pageSize) {
        this.pageSize = pageSize;
    }
}
