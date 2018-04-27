package com.microsoft.adaptivechatdemo;

import android.content.Context;
import android.support.annotation.Nullable;

import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.concurrent.TimeUnit;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.WebSocket;
import okhttp3.WebSocketListener;
import okio.ByteString;

public class MotherShipConnection {
    private MainActivity mUi;
    private WebSocket mWebsocket;

    MotherShipConnection(MainActivity activity)
    {
        mUi = activity;
    }

    public void EnumerateMotherships()
    {
        RequestQueue queue = Volley.newRequestQueue(mUi);
        String url ="https://cardfanout.azurewebsites.net/api/motherships";

        // Request a string response from the provided URL.
        StringRequest stringRequest = new StringRequest(com.android.volley.Request.Method.GET, url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        // Display the first 500 characters of the response string.
                        if (response != null &&  response.length()>2) {
                            String sub = response.substring(1, response.length() - 1);
                            ArrayList<String> list = new ArrayList<String>(Arrays.asList(sub.split(",")));
                            mUi.SendMotheshipsMessage(list);
                        }
                    }
                }, new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
                //TODO: handle error
            }
        });

        // Add the request to the RequestQueue.
        queue.add(stringRequest);

    }


    public void EstablishWebSocketConnection(String s) {
        OkHttpClient client = new OkHttpClient.Builder().readTimeout(0, TimeUnit.MILLISECONDS).build();
        String removedQuotes = s.substring(1, s.length()-1);
        Request request = new Request.Builder()
                .url("wss://cardfanout.azurewebsites.net/wsClient/"+removedQuotes)
                .build();

        mWebsocket = client.newWebSocket(request, new WebSocketListener() {
            @Override
            public void onOpen(WebSocket webSocket, okhttp3.Response response) {

                super.onOpen(webSocket, response);
            }

            @Override
            public void onMessage(WebSocket webSocket, String text) {
                try {
                    JSONObject json = new JSONObject(text);
                    if (json.getString("Type").compareTo("MothershipSendCard") == 0)
                    {

                        mUi.SendReceivedAdaptiveCard(json.getString("CardJson"));
                    }
                } catch (JSONException e) {
                    e.printStackTrace();
                }
            }

            @Override
            public void onMessage(WebSocket webSocket, ByteString bytes) {
                super.onMessage(webSocket, bytes);
            }

            @Override
            public void onClosing(WebSocket webSocket, int code, String reason) {
                super.onClosing(webSocket, code, reason);
            }

            @Override
            public void onClosed(WebSocket webSocket, int code, String reason) {
                super.onClosed(webSocket, code, reason);
            }

            @Override
            public void onFailure(WebSocket webSocket, Throwable t, @Nullable okhttp3.Response response) {
                super.onFailure(webSocket, t, response);
            }
        });
        client.dispatcher().executorService().shutdown();
    }

    public void closeSocket()
    {
        if(mWebsocket != null)
        {
            mWebsocket.close(0, "");
        }
    }
}
