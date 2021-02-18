# Introduction 
An Azure Function app that pulls metrics about Azure Service Bus subscriptions and pushes them to Application Insights.

# Getting Started
Most of the work is done in the ServiceBusNamespaceService class. This class uses the Microsoft.Azure.ServiceBus.Management namespace to discover all subscriptions in the provided ASB namespace, it then returns metrics on dead-letter and active message accounts for the subscriptions.

The SubscriptionMetricsTracker class then pushes the subscription metrics into App Insights as custom metrics.

The above steps are orchestrated by the MetricsCollector class.