# AdaptiveCardsFanOutDemo

Demo that fans out cards from a mothership to clients.


## How to build a client

The client needs to connect to a https website and then use web sockets to receive cards from the mothership.


### General client UX app flow

1. Display available motherships to connect to
2. When user selects a mothership, connect to it, and once received `ClientName`, go to the Chat page
3. When received mothership disconnected event or socket closed, go back to showing available motherships


### Step 1: Access all current motherships

Use a HTTP GET request to retrieve all current motherships.

Request URL: `https://cardfanout.azurewebsites.net/api/motherships`

Response: A JSON array of strings (mothership names).

```json
[ "Vulcan", "Tucson", "Miami" ]
```

Display these to the user in a list, letting them click the mothership they want to connect to.


### Step 2: Connect to the mothership

When a user selected a mothership, you now have the mothership name. You'll need that to connect. You'll have to open a web socket. Insert the mothership name in the URL as seen below.

Web Socket URL: `wss://cardfanout.azurewebsites.net/wsClient/[MOTERSHIPNAME]`


### Step 3: Receive messages from the mothership web socket

After you've connected to the web socket, you'll have to start receiving messages. You don't need to send messages, so no need to add code for that!

There's three messages you need to handle receiving... They're all JSON serialized, and the `Type` property lets you know which type of message they are.

#### ClientNameAssigned

This is the first message that you should receive immediately upon connecting to the web socket.

When you receive this, grab the `ClientName` you were assigned and then display the Chat page (you should display the `ClientName` at the top of the Chat page so that we can know which client you are).


```json
{
  "Type": "ClientNameAssigned",
  "ClientName": "[yourAssignedName]"
}
```

#### MothershipSendCard

When you receive this, display the card in the chat UI using the `CardJson`. You can ignore the `CardIdentifier`, it's currently not used in clients and probably won't ever be used.

```json
{
  "Type": "MothershipSendCard",
  "CardIdentifier": "[guid]",
  "CardJson": "[cardJson]"
}
```

#### MothershipDisconnected

When you receive this, navigate back to the connect to motherships page (and if possible, inform the user that the mothership was disconnected).

```json
{
  "Type": "MothershipDisconnected"
}
```
