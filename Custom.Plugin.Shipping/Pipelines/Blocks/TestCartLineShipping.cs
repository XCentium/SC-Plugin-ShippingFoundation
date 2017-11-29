using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Pipelines;

namespace Custom.Plugin.Shipping.Pipelines.Blocks
{

    /// <summary>
    /// 
    /// </summary>
    public class TestCartLineShipping : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {

            // If the cart is not null and has product lines
            if (arg != null && arg.Lines.Any())
            {

                // lop through to reset Shipping for all line that already has fulfilment set.
                foreach (var cartLineComponent in arg.Lines)
                {
                    // Get all adjustment applied to the cartline
                    var adjustments = cartLineComponent.Adjustments;

                    // It there are adjustments
                    if (adjustments != null && adjustments.Count > 0)
                    {

                        // Check if shipping adjustment has been applied by the default fulfillment plugin.
                        var fulfillment = adjustments.FirstOrDefault(x => x.Name.ToLower().Contains("fulfillmentfee"));

                        // If it has been applied, then we can simply replace it with our desired value based on our external APIs or internal apps.
                        if (fulfillment != null)
                        {
                            // Go get you shipping from Fedex or USPS or UPS web service or from Sitecore 
                            // This can come from calculating shipping from external source based on items in the cart above or address of the buyer
                            var customShipping = 0.99M;

                            // If already set, or same as the amount set return cart
                            if (customShipping == fulfillment.Adjustment.Amount)
                            {
                                continue;
                            }

                            // Prepare to overide the shipping value
                            var awardedAdjustment = new CartLineLevelAwardedAdjustment
                            {
                                Name = fulfillment.Name,
                                DisplayName = fulfillment.DisplayName
                            };

                            // convert the new shipping price from decimal to money
                            var money = new Money(context.CommerceContext.CurrentCurrency(), customShipping);

                            // set the money as the new adjustment
                            awardedAdjustment.Adjustment = money;


                            awardedAdjustment.AdjustmentType =
                                context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Fulfillment;

                            // populate other values with that of the default plugin value or change them to suit you.
                            awardedAdjustment.IsTaxable = fulfillment.IsTaxable;

                            // Set the name of the awarding block to that of the current code block
                            awardedAdjustment.AwardingBlock = Name;

                            // Remove the default shipping price
                            adjustments.Remove(fulfillment);

                            // Add the new shipping price
                            adjustments.Add(awardedAdjustment);

                        }
                    }
                }
            }

            // Return cart
            return await Task.FromResult(arg);
        }
    }


}
