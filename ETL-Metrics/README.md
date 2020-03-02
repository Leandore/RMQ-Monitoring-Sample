https://www.rabbitmq.com/monitoring.html#rabbitmq-metrics

https://rawcdn.githack.com/rabbitmq/rabbitmq-management/v3.7.19/priv/www/doc/stats.html


There's more than one way to consume messages, therefore there's more than one metrics: deliver_get is for basic.get
("pull" [1]), deliver is for deliveries to a consumer ("push" [3]) with a manual acknowledgement mode, deliver_no_ack is the same
thing but with automatic acknowledgements. I'd need to take a look at the code to see what "publish" is.


"confirm" and "return" are rates of publisher confirms and returns (which clarifies that you are looking at channel stats, something that wasn't explicitly stated).
If you don't use publisher confirms [2] and don't have unroutable messages published with the mandatory flag set to true, they won't be present/cannot be computed.

1. http://www.rabbitmq.com/api-guide.html#getting
2. http://www.rabbitmq.com/confirms.html
3. http://www.rabbitmq.com/api-guide.html#consuming
4. http://www.rabbitmq.com/api-guide.html#returning


https://docs.isus.emc.com/display/PCF/Application+Logging#ApplicationLogging-ELK