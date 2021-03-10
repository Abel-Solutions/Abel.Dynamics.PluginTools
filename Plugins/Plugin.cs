﻿using Microsoft.Xrm.Sdk;
using PluginBase;

namespace Plugins
{
    public class Plugin : PluginBase.PluginBase
    {
        public override void Execute(PluginContext context)
        {
            context.Trace("looool");

            var target = context.Target;
            var service = context.OrgService;

            target["name"] = "foo";

            var lol = new Entity("lol")
            {
                ["name"] = "bar"
            };
            var id = service.Create(lol);

            lol = service.RetrieveByAttribute("lol", "name", "bar2");

            lol.Id = id;
            lol["name"] = "bar2";
            service.Update(lol);
        }
    }
}