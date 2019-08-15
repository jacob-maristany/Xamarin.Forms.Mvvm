# Xamarin.Forms.Mvvm

## Messaging Example.. for you, David. ;) 

Note: The reason it's simpler/lighter than `MessagingCenter` is because `Messaging` does not need to hold the reference to the subscription. The subscriber is responsibile for that. This is a convention-based approach. 


#### Subscribing:
Make a private field scoped to the class to hold the reference of the delegate. The field and delegate should be in the same class/instance.

```
private Action<SomeParmType> SubscriptionResponseToken;`

private void SubscriptionResponse(SomeParamType p) { }
```

Store the delegate and subscribe:

```
SubscriptionResponseToken = SubscriptionResponse;
Messaging.Subscribe<SomeParamType>("channel_to_publsh_on", "unique_key_for_subscription", SubscriptionResponseToken);
```

#### Publishing:
```
Messaging.Publish<SomeType>("channel_to_publsh_on", SomeInstanceOfType);
```

Simpler API surface for a BaseViewModel. Totally optional.

```
protected Messaging Messaging => Messaging.Instance;
```
