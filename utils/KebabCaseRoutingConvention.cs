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
                // 将控制器的路由转换为小写中划线
                foreach (var selector in controller.Selectors)
                {
                    if (selector.AttributeRouteModel != null)
                    {
                        selector.AttributeRouteModel.Template = ConvertToKebabCase(selector.AttributeRouteModel.Template);
                    }
                }

                // 遍历控制器中的每个动作，将其路由转换为小写中划线
                foreach (var action in controller.Actions)
                {
                    foreach (var selector in action.Selectors)
                    {
                        if (selector.AttributeRouteModel != null)
                        {
                            selector.AttributeRouteModel.Template = ConvertToKebabCase(selector.AttributeRouteModel.Template);
                        }
                    }
                }
            }
        }
        // 转换为小写中划线的逻辑
        private string ConvertToKebabCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 使用正则表达式将大写字母转换为小写字母并加上中划线
            var kebabCase = Regex.Replace(input, "([a-z])([A-Z])", "$1-$2").ToLower();
            return kebabCase;
        }

    }
}