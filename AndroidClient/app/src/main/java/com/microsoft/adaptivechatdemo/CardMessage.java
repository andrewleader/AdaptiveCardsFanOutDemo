package com.microsoft.adaptivechatdemo;

import android.view.View;

import io.adaptivecards.objectmodel.AdaptiveCard;

/**
 * Created by eschavez on 4/13/2018.
 */

public class CardMessage {

    private String message;
    private AdaptiveCard adaptiveCard;
    private View cardView;
    private boolean isReceived;
    private long createdAt;

    public CardMessage(String message, long time) {
        this.message = message;
        this.createdAt = time;
    }

    public long getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(long createdAt) {
        this.createdAt = createdAt;
    }

    public boolean isReceived() {
        return isReceived;
    }

    public void setReceived(boolean received) {
        isReceived = received;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public AdaptiveCard getAdaptiveCard() {
        return adaptiveCard;
    }

    public void setAdaptiveCard(AdaptiveCard adaptiveCard) {
        this.adaptiveCard = adaptiveCard;
    }

    public View getCardView() {
        return cardView;
    }

    public void setCardView(View cardView) {
        this.cardView = cardView;
    }
}
