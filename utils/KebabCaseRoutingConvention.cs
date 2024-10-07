using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace ParkingManagementAPI.utils
{
   public class KebabCaseRoutingConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            // 获取控制器名称
            var controllerName = ConvertToKebabCase(controller.ControllerName);

            // 处理控制器的路由
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    // 如果模板包含占位符 [controller] 或 [action]，替换为实际名称
                    selector.AttributeRouteModel.Template = ReplaceTokens(selector.AttributeRouteModel.Template, controllerName, null);
                }
            }

            // 处理控制器中的每个动作
            foreach (var action in controller.Actions)
            {
                // 获取动作名称
                var actionName = ConvertToKebabCase(action.ActionName);

                foreach (var selector in action.Selectors)
                {
                    if (selector.AttributeRouteModel != null)
                    {
                        // 同样替换 [controller] 和 [action] 占位符
                        selector.AttributeRouteModel.Template = ReplaceTokens(selector.AttributeRouteModel.Template, controllerName, actionName);
                    }
                   
                }
            }
        }
    }

    // 替换 [controller] 和 [action] 占位符的方法
    private string ReplaceTokens(string template, string controllerName, string actionName)
    {
        if (string.IsNullOrEmpty(template))
            return template;

        // 替换 [controller] 和 [action] 占位符
        template = template.Replace("[controller]", controllerName);

        if (!string.IsNullOrEmpty(actionName))
        {
            template = template.Replace("[action]", actionName);
        }

        return template;
    }

    // 转换为小写中划线的逻辑
    private string ConvertToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // 正则表达式处理连续大写字母以及大写字母和小写字母的转换
        var kebabCase = Regex.Replace(input, "([a-z0-9])([A-Z])", "$1-$2").ToLower();
        return kebabCase;
    }
}

}