package com.microsoft.adaptivechatdemo;

import android.content.DialogInterface;
import android.os.Handler;
import android.os.Message;
import android.support.v7.app.AlertDialog;
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
    private static final int SOCKET_CLOSED = 4;
    private static final int NAME_ASSIGNED = 5;

    private static int num = 0;

    public void SendMotheshipsMessage(ArrayList<CharSequence> motherships)
    {
        Message msg = new Message();
        msg.what = MOTHERSHIPS_UPDATED;
        msg.getData().putCharSequenceArray("motherships", motherships.toArray(new CharSequence[0]));
        mHandler.sendMessage(msg);
    }

    public void SendNamedAssigned(String name)
    {
        Message msg = new Message();
        msg.what = NAME_ASSIGNED;
        msg.getData().putString("Name", name );
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

    public void SendSocketClosed() {
        Message msg = new Message();
        msg.what = SOCKET_CLOSED;
        mHandler.sendMessage(msg);
    }

    class ItemChanges extends Handler
    {

        @Override
        public void handleMessage(Message msg)
        {
            switch(msg.what){
                case (MOTHERSHIPS_UPDATED):
                    AlertDialog.Builder b = new AlertDialog.Builder(MainActivity.this);
                    b.setTitle("Choose a connection:");
                    final CharSequence[] connections = (msg.getData().getCharSequenceArray("motherships"));
                    b.setItems(connections, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int which) {
                            dialog.dismiss();
                            mConnection.EstablishWebSocketConnection(connections[which].toString());
                        }
                    });
                    b.show();
                    break;
                case (ITEM_ADDED):
                    int num = msg.arg1;
                    mMessageAdapter.notifyItemInserted(num);
                    mMessageRecycler.smoothScrollBy(0, Integer.MAX_VALUE);
                    break;
                case(ITEM_REMOVED):
                    int removed = msg.arg1;
                    mMessageAdapter.notifyItemRemoved(removed);
                    break;
                case(SOCKET_CLOSED):
                    mConnection.enumerateMotherships();
                    break;
                case(NAME_ASSIGNED):
                    //TODO: Display name
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

        /*button.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                mMessageAdapter.addNewSentMessage(mInput.getText().toString());
                mInput.setText("");
                mMessageAdapter.notifyItemInserted(mMessageAdapter.getItemCount()-1);
                mMessageRecycler.smoothScrollToPosition(mMessageAdapter.getItemCount()-1);
            }
        });*/

        mMessageRecycler = (RecyclerView) findViewById(R.id.reyclerview_message_list);
        mMessageAdapter = new MessageListAdapter(this, list, this.getSupportFragmentManager());

        // Scroll down when size of any of the cards changes (they can change as images load)
        mMessageAdapter.addOnLayoutChangeListener(new View.OnLayoutChangeListener() {
            @Override
            public void onLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom) {
                mMessageRecycler.smoothScrollBy(0, Integer.MAX_VALUE);
            }
        });

        mMessageRecycler.setLayoutManager(new LinearLayoutManager(this));

        mMessageRecycler.setItemAnimator(new ScaleInBottomAnimator(new OvershootInterpolator(1f)));
        mMessageRecycler.getItemAnimator().setAddDuration(100);
        mMessageRecycler.getItemAnimator().setMoveDuration(0);
        mMessageRecycler.setAdapter(mMessageAdapter);

        mConnection = new MotherShipConnection(this);
        mConnection.enumerateMotherships();
    }

    @Override
    protected void onDestroy()
    {
        mConnection.closeSocket();
        super.onDestroy();
    }
}
