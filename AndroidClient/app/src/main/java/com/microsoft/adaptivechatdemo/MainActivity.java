package com.microsoft.adaptivechatdemo;

import android.os.Handler;
import android.os.Message;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.View;
import android.view.animation.OvershootInterpolator;
import android.widget.Button;
import android.widget.EditText;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Timer;
import java.util.TimerTask;

import io.adaptivecards.objectmodel.AdaptiveCard;
import io.adaptivecards.objectmodel.ParseResult;
import io.adaptivecards.renderer.AdaptiveCardRenderer;
import jp.wasabeef.recyclerview.animators.ScaleInBottomAnimator;

public class MainActivity extends AppCompatActivity {
    private static RecyclerView mMessageRecycler;
    private static MessageListAdapter mMessageAdapter;
    private EditText mInput;
    private Timer mTimer;
    private Handler mHandler;
    private MotherShipConnection mConnection;

    private static final int ITEM_ADDED = 1;
    private static final int ITEM_REMOVED = 2;
    private static final int MOTHERSHIPS_UPDATED = 3;

    private static int num = 0;

    public void SendMotheshipsMessage(ArrayList<String> motherships)
    {
        //TODO: show a picker to the UI;

        if (motherships.size() >= 1)
        {
            mConnection.EstablishWebSocketConnection(motherships.get(0));
        }

        Message msg = new Message();
        msg.what = MOTHERSHIPS_UPDATED;
        msg.obj = motherships;
        mHandler.sendMessage(msg);
    }

    public void SendReceivedAdaptiveCard(String jsonPayload)
    {
        CardMessage message = new CardMessage("", Calendar.getInstance().getTimeInMillis());
        message.setReceived(true);

        try {
            ParseResult adaptiveCard = AdaptiveCard.DeserializeFromString(jsonPayload, AdaptiveCardRenderer.VERSION);
            message.setAdaptiveCard(adaptiveCard.GetAdaptiveCard());
        } catch (IOException e) {
            e.printStackTrace();
        }

        mMessageAdapter.addNewMessage(message);

        Message msg = new Message();
        msg.what = ITEM_ADDED;
        msg.arg1 = mMessageAdapter.getItemCount()-1;
        mHandler.sendMessage(msg);
    }

    static class ItemChanges extends Handler
    {

        @Override
        public void handleMessage(Message msg)
        {
            switch(msg.what){
                case (ITEM_ADDED):
                    int num = msg.arg1;
                    mMessageAdapter.notifyItemInserted(num);
                    mMessageRecycler.smoothScrollToPosition(num);
                    break;
                case(ITEM_REMOVED):
                    int removed = msg.arg1;
                    mMessageAdapter.notifyItemRemoved(removed);
                    break;
            }

        }
    }

    private TimerTask task;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        mHandler = new ItemChanges();

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_message_list);

        mInput = findViewById(R.id.edittext_chatbox);
        Button button = findViewById(R.id.button_chatbox_send);

        ArrayList<CardMessage> list = new ArrayList<>();

        button.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                mMessageAdapter.addNewSentMessage(mInput.getText().toString());
                mInput.setText("");
                mMessageAdapter.notifyItemInserted(mMessageAdapter.getItemCount()-1);
                mMessageRecycler.smoothScrollToPosition(mMessageAdapter.getItemCount()-1);
            }
        });

        mMessageRecycler = (RecyclerView) findViewById(R.id.reyclerview_message_list);
        mMessageAdapter = new MessageListAdapter(this, list, this.getSupportFragmentManager());
        mMessageRecycler.setLayoutManager(new LinearLayoutManager(this));

        mMessageRecycler.setItemAnimator(new ScaleInBottomAnimator(new OvershootInterpolator(1f)));
        mMessageRecycler.getItemAnimator().setAddDuration(1000);
        mMessageRecycler.getItemAnimator().setMoveDuration(0);
        mMessageRecycler.setAdapter(mMessageAdapter);

        mConnection = new MotherShipConnection(this);
        mConnection.EnumerateMotherships();
    }

    @Override
    protected void onDestroy()
    {
        mConnection.closeSocket();
        super.onDestroy();
    }
}
