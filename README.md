# Xamarin.Forms.Mvvm

Messaging Example.. for you, David. ;) 

Note: The reason it's simpler/lighter than MessagingCenter is because the MessagingCenter does not need to hold the reference to the subscription. The subscriber is responsibile for that. It's convention-based. 


Subscribing:
Make a private field scoped to the class to hold the reference of the delegate. Field and delegate should be in the same class/instance.

`private Action<SomeParmType> SubscriptionResponseAction;`

`private void SubscriptionResponse(SomeParamType p) { }`
        
Store the delegate and subscribe:
```
SubscriptionResponseAction = SubscriptionResponse;
Messaging.Subscribe<SomeParamType>("channel_to_publsh_on", "unique_key_for_subscription", SubscriptionResponseAction);
```
Publishing:

`Messaging.Publish<SomeType>("channel_to_publsh_on", SomeInstanceOfType);`
