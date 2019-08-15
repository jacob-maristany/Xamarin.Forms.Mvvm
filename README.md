# Xamarin.Forms.Mvvm

Messaging Example.. for you, David. ;) 


Subscribing:
Make a private field scoped to the class to hold the reference of the delegate. Field and delegate should be in the same class/instance.

private Action<SomeParmType> SubscriptionResponseAction;

private void SubscriptionResponse(SomeParamType p) { }
        
Store the delegate and subscribe:

SubscriptionResponseAction = SubscriptionResponse;
Messaging.Subscribe<SomeParamType>("channel_to_publsh_on", "unique_key_for_subscription", SubscriptionResponseAction);

Publishing:

Messaging.Publish<SomeType>("channel_to_publsh_on", SomeInstanceOfType);
