
Sitecore Commerce Engine Custom Shipping plugin (Sitecore Commerce 9)
======================================

This plugin allows the user to override default Sitecore Commerce Engine Fulfillment. It can be used as a basis for your custom shipping price. 
- It is very easy to integrate or extend to fit your needs.
- Using this plugin, you will be able to integrate with external shipping sources such as FEDEX, USPS,UPS, Sitecore Lookup table, other external shipping APIs.



Sponsor
=======
This plugin was sponsored and created by Xcentium.

How to Install
==============

1. Copy it to your Sitecore Commerce Engine Solution and add it as a project 


2. Add it as a dependency to your Adventure works or Habitat or Custom Site plugin' project

3. To your ServiceCollectionExtensions.cs class, replace:
```
.Add<CalculateCartLinesFulfillmentBlock>()

With:

.Add<TestCartLineShipping>().After<CalculateCartLinesFulfillmentBlock>()

and

.Add<CalculateCartFulfillmentBlock>()

with

.Add<TestCartShipping>().After<CalculateCartFulfillmentBlock>()
```
	So that the config section looks somewhat like below:

```
        public static IServiceCollection ConfigureCartPipelines(this IServiceCollection services)
        {
            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<ICalculateCartLinesPipeline>(builder => builder
                    .Add<PopulateCartLineItemsBlock>()
                    .Add<CalculateCartLinesPriceBlock>()
                    .Add<ValidateCartLinesPriceBlock>()
                    .Add<CalculateCartLinesSubTotalsBlock>()
                    //.Add<CalculateCartLinesFulfillmentBlock>()
                    .Add<TestCartLineShipping>().After<CalculateCartLinesFulfillmentBlock>()
                    .Add<ValidateCartCouponsBlock>()
                    .Add<CalculateCartLinesPromotionsBlock>()
                    .Add<CalculateCartLinesTaxBlock>()
                    .Add<CalculateCartLinesTotalsBlock>())

               .ConfigurePipeline<ICalculateCartPipeline>(builder => builder
                    .Add<CalculateCartSubTotalsBlock>()
                    //.Add<CalculateCartFulfillmentBlock>()
                    .Add<TestCartShipping>().After<CalculateCartFulfillmentBlock>()
                    .Add<CalculateCartPromotionsBlock>()
                    .Add<CalculateCartTaxBlock>()
                    .Add<CalculateCartTotalsBlock>()
                    .Add<CalculateCartPaymentsBlock>())
                    
              .ConfigurePipeline<IAddPaymentsPipeline>(builder => builder.Add<ValidateCartHasFulfillmentBlock>().After<ValidateCartAndPaymentsBlock>())
                            
              .ConfigurePipeline<IOrderPlacedPipeline>(builder => builder.Replace<OrderPlacedAssignConfirmationIdBlock, CustomOrderNumber>())

              );

            return services;
        }

```

4. It is now ready to use. It will start overiding the default shipping price with your custom shipping price. 

Note:
=====

- If you have any questions, comment or need us to help install, extend or adapt to your needs, do not hesitate to reachout to us at XCentium.




