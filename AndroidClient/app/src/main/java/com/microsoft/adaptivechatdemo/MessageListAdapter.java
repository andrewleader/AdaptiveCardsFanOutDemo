package com.microsoft.adaptivechatdemo;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.support.v4.app.FragmentManager;
import android.support.v7.widget.RecyclerView;
import android.text.format.DateUtils;
import android.view.ContextThemeWrapper;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.Map;

import io.adaptivecards.objectmodel.ActionType;
import io.adaptivecards.objectmodel.BaseActionElement;
import io.adaptivecards.objectmodel.HostConfig;
import io.adaptivecards.objectmodel.OpenUrlAction;
import io.adaptivecards.objectmodel.ShowCardAction;
import io.adaptivecards.objectmodel.SubmitAction;
import io.adaptivecards.renderer.AdaptiveCardRenderer;
import io.adaptivecards.renderer.RenderedAdaptiveCard;
import io.adaptivecards.renderer.actionhandler.ICardActionHandler;

/**
 * Created by eschavez on 4/13/2018.
 */


public class MessageListAdapter extends RecyclerView.Adapter {
    private static final int VIEW_TYPE_MESSAGE_SENT = 1;
    private static final int VIEW_TYPE_MESSAGE_RECEIVED = 2;

    private Context mContext;
    private FragmentManager mFragmentManager;
    private List<CardMessage> mMessageList;

    public HostConfig hostConfig;

    private final String HOST_CONFIG =
            "{\n" +
                    "  \"spacing\": {\n" +
                    "    \"small\": 3,\n" +
                    "    \"default\": 8,\n" +
                    "    \"medium\": 20,\n" +
                    "    \"large\": 30,\n" +
                    "    \"extraLarge\": 40,\n" +
                    "    \"padding\": 20\n" +
                    "  },\n" +
                    "  \"separator\": {\n" +
                    "    \"lineThickness\": 1,\n" +
                    "    \"lineColor\": \"#000000\"\n" +
                    "  },\n" +
                    "  \"supportsInteractivity\": true,\n" +
                    "  \"fontFamily\": \"Segoe UI\",\n" +
                    "  \"fontSizes\": {\n" +
                    "    \"small\": 10,\n" +
                    "    \"default\": 12,\n" +
                    "    \"medium\": 14,\n" +
                    "    \"large\": 17,\n" +
                    "    \"extraLarge\": 20\n" +
                    "  },\n" +
                    "  \"fontWeights\": {\n" +
                    "    \"lighter\": 200,\n" +
                    "    \"default\": 400,\n" +
                    "    \"bolder\": 600\n" +
                    "  },\n" +
                    "  \"containerStyles\": {\n" +
                    "    \"default\": {\n" +
                    "      \"backgroundColor\": \"#00000000\",\n" +
                    "      \"foregroundColors\": {\n" +
                    "        \"default\": {\n" +
                    "          \"default\": \"#FF333333\",\n" +
                    "          \"subtle\": \"#EE333333\"\n" +
                    "        },\n" +
                    "        \"accent\": {\n" +
                    "          \"default\": \"#FF2E89FC\",\n" +
                    "          \"subtle\": \"#882E89FC\"\n" +
                    "        },\n" +
                    "        \"attention\": {\n" +
                    "          \"default\": \"#FFcc3300\",\n" +
                    "          \"subtle\": \"#DDcc3300\"\n" +
                    "        },\n" +
                    "        \"good\": {\n" +
                    "          \"default\": \"#FF54a254\",\n" +
                    "          \"subtle\": \"#DD54a254\"\n" +
                    "        },\n" +
                    "        \"warning\": {\n" +
                    "          \"default\": \"#FFe69500\",\n" +
                    "          \"subtle\": \"#DDe69500\"\n" +
                    "        }\n" +
                    "      }\n" +
                    "    },\n" +
                    "    \"emphasis\": {\n" +
                    "      \"backgroundColor\": \"#08000000\",\n" +
                    "      \"foregroundColors\": {\n" +
                    "        \"default\": {\n" +
                    "          \"default\": \"#FF333333\",\n" +
                    "          \"subtle\": \"#EE333333\"\n" +
                    "        },\n" +
                    "        \"accent\": {\n" +
                    "          \"default\": \"#FF2E89FC\",\n" +
                    "          \"subtle\": \"#882E89FC\"\n" +
                    "        },\n" +
                    "        \"attention\": {\n" +
                    "          \"default\": \"#FFcc3300\",\n" +
                    "          \"subtle\": \"#DDcc3300\"\n" +
                    "        },\n" +
                    "        \"good\": {\n" +
                    "          \"default\": \"#FF54a254\",\n" +
                    "          \"subtle\": \"#DD54a254\"\n" +
                    "        },\n" +
                    "        \"warning\": {\n" +
                    "          \"default\": \"#FFe69500\",\n" +
                    "          \"subtle\": \"#DDe69500\"\n" +
                    "        }\n" +
                    "      }\n" +
                    "    }\n" +
                    "  },\n" +
                    "  \"imageSizes\": {\n" +
                    "    \"small\": 20,\n" +
                    "    \"medium\": 40,\n" +
                    "    \"large\": 80\n" +
                    "  },\n" +
                    "  \"actions\": {\n" +
                    "    \"maxActions\": 5,\n" +
                    "    \"spacing\": \"default\",\n" +
                    "    \"buttonSpacing\": 10,\n" +
                    "    \"showCard\": {\n" +
                    "      \"actionMode\": \"inline\",\n" +
                    "      \"inlineTopMargin\": 16,\n" +
                    "      \"style\":\"emphasis\"\n" +
                    "    },\n" +
                    "    \"actionsOrientation\": \"horizontal\",\n" +
                    "    \"actionAlignment\": \"stretch\"\n" +
                    "  },\n" +
                    "  \"adaptiveCard\": {\n" +
                    "    \"allowCustomStyle\": false\n" +
                    "  },\n" +
                    "  \"imageSet\": {\n" +
                    "    \"imageSize\": \"medium\",\n" +
                    "    \"maxImageHeight\": 100\n" +
                    "  },\n" +
                    "  \"factSet\": {\n" +
                    "    \"title\": {\n" +
                    "      \"color\": \"default\",\n" +
                    "      \"size\": \"default\",\n" +
                    "      \"isSubtle\": false,\n" +
                    "      \"weight\": \"bolder\",\n" +
                    "      \"wrap\": true,\n" +
                    "      \"maxWidth\": 150\n" +
                    "    },\n" +
                    "    \"value\": {\n" +
                    "      \"color\": \"default\",\n" +
                    "      \"size\": \"default\",\n" +
                    "      \"isSubtle\": false,\n" +
                    "      \"weight\": \"default\",\n" +
                    "      \"wrap\": true\n" +
                    "    },\n" +
                    "    \"spacing\": 10\n" +
                    "  }\n" +
                    "}";

    public MessageListAdapter(Context context, List<CardMessage> cards, FragmentManager fragmentManager) {
        mContext = new ContextThemeWrapper(context, R.style.RecyclerViewTheme);
        mFragmentManager = fragmentManager;
        mMessageList = new ArrayList<>();
        mMessageList = cards;
        hostConfig = HostConfig.DeserializeFromString(HOST_CONFIG);
    }

    public void addNewSentMessage(String message)
    {
        CardMessage cardMessage = new CardMessage(message, Calendar.getInstance().getTimeInMillis());
        cardMessage.setReceived(false);
        mMessageList.add(cardMessage);
    }

    public void addNewMessage(CardMessage message)
    {
        mMessageList.add(message);
    }

    @Override
    public int getItemCount() {
        return mMessageList.size();
    }

    // Determines the appropriate ViewType according to the sender of the message.
    @Override
    public int getItemViewType(int position) {
        CardMessage message = mMessageList.get(position);

        if (!message.isReceived()) {
            // If the current user is the sender of the message
            return VIEW_TYPE_MESSAGE_SENT;
        } else {
            // If some other user sent the message
            return VIEW_TYPE_MESSAGE_RECEIVED;
        }
    }

    // Inflates the appropriate layout according to the ViewType.
    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view;

        if (viewType == VIEW_TYPE_MESSAGE_SENT) {
            view = LayoutInflater.from(parent.getContext())
                    .inflate(R.layout.item_message_sent, parent, false);
            return new SentMessageHolder(view);
        } else if (viewType == VIEW_TYPE_MESSAGE_RECEIVED) {
            view = LayoutInflater.from(parent.getContext())
                    .inflate(R.layout.item_message_received, parent, false);
            return new ReceivedMessageHolder(view);
        }

        return null;
    }

    // Passes the message object to a ViewHolder so that the contents can be bound to UI.
    @Override
    public void onBindViewHolder(RecyclerView.ViewHolder holder, int position) {
        CardMessage message = mMessageList.get(position);

        switch (holder.getItemViewType()) {
            case VIEW_TYPE_MESSAGE_SENT:
                ((SentMessageHolder) holder).bind(message);
                break;
            case VIEW_TYPE_MESSAGE_RECEIVED:
                ((ReceivedMessageHolder) holder).bind(message);
        }
    }

    private class SentMessageHolder extends RecyclerView.ViewHolder {
        TextView messageText, timeText;

        SentMessageHolder(View itemView) {
            super(itemView);

            messageText = (TextView) itemView.findViewById(R.id.text_message_body);
            timeText = (TextView) itemView.findViewById(R.id.text_message_time);
        }

        void bind(CardMessage message) {
            messageText.setText(message.getMessage());

            // Format the stored timestamp into a readable String using method.
            timeText.setText(DateUtils.formatDateTime(mContext, message.getCreatedAt(), DateUtils.FORMAT_SHOW_TIME));
        }
    }

    private class ReceivedMessageHolder extends RecyclerView.ViewHolder  implements ICardActionHandler {
        TextView timeText, nameText;
        LinearLayout linearLayout;


        private void onOpenUrl(BaseActionElement actionElement)
        {
            OpenUrlAction openUrlAction = null;
            if (actionElement instanceof ShowCardAction)
            {
                openUrlAction = (OpenUrlAction) actionElement;
            }
            else if ((openUrlAction = OpenUrlAction.dynamic_cast(actionElement)) == null)
            {
                throw new InternalError("Unable to convert BaseActionElement to ShowCardAction object model.");
            }

            Intent browserIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(openUrlAction.GetUrl()));
            mContext.startActivity(browserIntent);
        }

        private void onSubmit(BaseActionElement actionElement, RenderedAdaptiveCard renderedAdaptiveCard) {
            SubmitAction submitAction = null;
            if (actionElement instanceof SubmitAction) {
                submitAction = (SubmitAction) actionElement;
            } else if ((submitAction = SubmitAction.dynamic_cast(actionElement)) == null) {
                throw new InternalError("Unable to convert BaseActionElement to ShowCardAction object model.");
            }

            String data = submitAction.GetDataJson();
            Map<String, String> keyValueMap = renderedAdaptiveCard.getInputs();

            String dataString = "Failed";
            if (!data.isEmpty())
            {
                try {
                    JSONObject object = new JSONObject(data);
                    dataString = "Submit data: " + object.toString() + "\nInput: " + keyValueMap.toString();
                } catch (JSONException e) {
                    //showToast(e.toString(), Toast.LENGTH_LONG);
                }
            }
            else
            {
                dataString = "Submit input: " + keyValueMap.toString();
            }
            CardMessage message = new CardMessage(dataString, Calendar.getInstance().getTimeInMillis());
            mMessageList.add(message);
        }

        @Override
        public void onAction(BaseActionElement actionElement, RenderedAdaptiveCard renderedCard)
        {
            ActionType actionType = actionElement.GetElementType();
            if (actionType == ActionType.Submit)
            {
                onSubmit(actionElement, renderedCard);
            }
            else if (actionType == ActionType.OpenUrl)
            {
                onOpenUrl(actionElement);
            }
        }

        ReceivedMessageHolder(View itemView) {
            super(itemView);

            timeText = (TextView) itemView.findViewById(R.id.text_message_time);
            nameText = (TextView) itemView.findViewById(R.id.text_message_name);
            linearLayout = (LinearLayout) itemView.findViewById(R.id.chat_card_view);
        }

        void bind(CardMessage message) {

            // Format the stored timestamp into a readable String using method.
            timeText.setText(DateUtils.formatDateTime(mContext, message.getCreatedAt(), DateUtils.FORMAT_SHOW_TIME));

            View cardView = message.getCardView();

            if (cardView == null) {
                RenderedAdaptiveCard renderedCard = AdaptiveCardRenderer.getInstance().render(mContext, mFragmentManager, message.getAdaptiveCard(), this, hostConfig);
                cardView = renderedCard.getView();

                message.setCardView(cardView);
            }
            linearLayout.removeAllViews();
            if (cardView.getParent() != null)
            {
                ((ViewGroup)cardView.getParent()).removeView(cardView);
            }
            linearLayout.addView(cardView);
        }
    }
}