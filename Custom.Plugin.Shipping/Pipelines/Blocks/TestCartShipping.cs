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
    public class TestCartShipping : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            // Get all the adjustments that may have been applied.
            var adjustments = arg.Adjustments;

            // If no adjustments return cart
            if (adjustments == null || adjustments.Count <= 0)
            {
                return await Task.FromResult(arg);
            }

            // Check if an adjustment has been applied by the default fulfilment plugin that is what we need to overide.
            var fulfillment = adjustments.FirstOrDefault(x => x.Name.ToLower().Contains("fulfillmentfee"));

            if (fulfillment == null)
            {
                return await Task.FromResult(arg);
            }


            // If you are at this point, the default fulfillment plugin has applied an adjustment. and we can now replace it with our own custom.

            // Go get you shipping from Fedex or USPS or UPS web service or from Sitecore 
            var customShipping = 6.95M; // This can come from calculating shipping from external source based on items in the cart above or address of the buyer

            // If already set, or same as the amount set return cart
            if (customShipping == fulfillment.Adjustment.Amount)
            {
                return await Task.FromResult(arg);
            }

            // Prepare to overide the shipping value
            var awardedAdjustment = new CartLevelAwardedAdjustment
            {
                Name = fulfillment.Name,
                DisplayName = fulfillment.DisplayName
            };

            // convert the new shipping price from decimal to money
            var money = new Money(context.CommerceContext.CurrentCurrency(), customShipping);

            // set the money as the new adjustment
            awardedAdjustment.Adjustment = money;

            awardedAdjustment.AdjustmentType = context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Fulfillment;

            // populate other values with that of the default plugin value or change them to suit you.
            awardedAdjustment.IsTaxable = fulfillment.IsTaxable;

            // Set the name of the awarding block to that of the current code block
            awardedAdjustment.AwardingBlock = Name;

            // Remove the default shipping price
            adjustments.Remove(fulfillment);

            // Add the new shipping price
            adjustments.Add(awardedAdjustment);

            // Return cart
            return await Task.FromResult(arg);
        }
    }

}
