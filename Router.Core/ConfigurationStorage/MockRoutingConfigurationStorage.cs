using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Router.Core.Model.Configuration;

namespace Router.Core.ConfigurationStorage
{
    public class MockRoutingConfigurationStorage : IRoutingConfigurationStorage
    {
        private readonly List<RoutingConfigurationEntry> _rules;
        private readonly List<RouteTargetConfigurationEntry> _targets;

        public MockRoutingConfigurationStorage()
        {
            _rules = DefaultRoutingConfigurationProvider.RoutingConfiguration.Entries.ToList();
            _targets = DefaultRoutingConfigurationProvider.RouteTargetConfiguration.Entries.ToList();
        }

        public Task<RoutingConfiguration> GetRoutingConfiguration()
        {
            return Task.FromResult(new RoutingConfiguration(_rules.ToArray()));
        }

        public Task SetRoutingConfiguration(RoutingConfiguration configuration)
        {
            _rules.Clear();
            _rules.AddRange(configuration.Entries);
            return Task.CompletedTask;
        }

        public Task<RouteTargetConfiguration> GetRouteTargetConfiguration()
        {
            return Task.FromResult(new RouteTargetConfiguration(_targets.ToArray()));
        }

        public Task SetRouteTargetConfiguration(RouteTargetConfiguration configuration)
        {
            _targets.Clear();
            _targets.AddRange(configuration.Entries);
            return Task.CompletedTask;
        }

        public Task BeginServerMaintenance(string target)
        {
            var index = _targets.FindIndex(x => x.Target == target);
            if (index >= 0)
                _targets[index] = new RouteTargetConfigurationEntry(_targets[index].Target, _targets[index].Address, true);
            return Task.CompletedTask;
        }

        public Task FinishServerMaintenance(string target)
        {
            var index = _targets.FindIndex(x => x.Target == target);
            if (index >= 0)
                _targets[index] = new RouteTargetConfigurationEntry(_targets[index].Target, _targets[index].Address, false);
            return Task.CompletedTask;
        }

        public Task<RouteTargetConfigurationEntry> GetRouteTarget(string target)
        {
            return Task.FromResult(_targets.First(x => x.Target == target));
        }

        public Task<RoutingConfigurationEntry[]> GetRouting(string serverType)
        {
            return Task.FromResult(_rules.Where(x => x.Server == serverType).ToArray());
        }

        public Task<string> AddRoutingRule(RoutingConfigurationEntry entry)
        {
            var id = Guid.NewGuid().ToString();
            var newEntry = new RoutingConfigurationEntry(id, entry.Server, entry.Platform, entry.ClientVersion, entry.RouteTarget, entry.UpdateMode);
            _rules.Add(newEntry);
            return Task.FromResult(id);
        }

        public Task UpdateRoutingRule(string id, RoutingConfigurationEntry entry)
        {
            var index = _rules.FindIndex(x => x.Id == id);
            if (index >= 0)
                _rules[index] = new RoutingConfigurationEntry(id, entry.Server, entry.Platform, entry.ClientVersion, entry.RouteTarget, entry.UpdateMode);
            return Task.CompletedTask;
        }

        public Task DeleteRoutingRule(string id)
        {
            _rules.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }

        public Task AddRouteTarget(RouteTargetConfigurationEntry entry)
        {
            _targets.Add(entry);
            return Task.CompletedTask;
        }

        public Task UpdateRouteTarget(string oldTarget, RouteTargetConfigurationEntry entry)
        {
            var index = _targets.FindIndex(x => x.Target == oldTarget);
            if (index >= 0)
                _targets[index] = entry;
            return Task.CompletedTask;
        }

        public Task DeleteRouteTarget(string target)
        {
            _targets.RemoveAll(x => x.Target == target);
            return Task.CompletedTask;
        }
    }
}
