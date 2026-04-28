# Router Server API Reference

Base URL: `http://localhost:5000`
All routes are prefixed with `Prod/`.
Every response uses HTTP 200 — the operation status is delivered through the `Code` field of the response body.
String fields in JSON requests and responses are lowercased automatically (`LowercaseStringConverter`), with the exception of target `Address`, which is preserved as-is.

---

## Authorization

All endpoints except `get-server-address` and `/health` require the header:

```
Authorization: Token
```

The token value is taken from `RouterConfiguration.Token` (file `.config/app.json`). On a missing or wrong token the server replies with HTTP 401 and an empty body.

---

## Reserved `maintenance` target

On every startup the server checks whether a target named `maintenance` exists in the configuration and creates it automatically if it is missing (with `Address = http://maintenance` and `Maintenance = true`). This guarantees that there is always a target available to switch routing to during downtime — you cannot delete it through normal operations as long as a rule references it.

---

## Data types

### RoutingConfigurationEntry
```json
{
  "id": "guid-string",
  "server": "string",
  "platform": "string",
  "clientVersion": "string (X.Y.Z format)",
  "routeTarget": "string",
  "updateMode": "UpToDate | UpdateRecommended | UpdateRequiredOfflineAllowed | UpdateRequired"
}
```

### RouteTargetConfigurationEntry
```json
{
  "target": "string",
  "address": "string (http/https URL)",
  "maintenance": "bool"
}
```

### UpdateMode enum
`None` (not valid in requests) | `UpToDate` | `UpdateRecommended` | `UpdateRequiredOfflineAllowed` | `UpdateRequired`

---

## Reading the configuration

### GET `Prod/get-routing-configuration`
Returns all routing rules. Each rule carries an `id` (GUID).

**Response:**
```json
{ "code": "Ok", "configuration": { "entries": [ RoutingConfigurationEntry, ... ] } }
```
Codes: `Ok` | `InternalServerError`

---

### GET `Prod/get-route-target-configuration`
Returns all route targets.

**Response:**
```json
{ "code": "Ok", "configuration": { "entries": [ RouteTargetConfigurationEntry, ... ] } }
```
Codes: `Ok` | `InternalServerError`

---

### POST `Prod/get-server-address`
Public endpoint (no authorization). The client uses it to discover the address of the backend it should talk to.

**Request:**
```json
{ "Server": "string", "ClientPlatform": "string", "ClientVersion": "X.Y.Z" }
```

**Response:**
```json
{ "code": "Ok", "serverAddress": "http://..." }
```

The address is only returned when the client is allowed to connect — i.e. for `Ok` and `UpdateRecommended`. For `UpdateRequired`, `UpdateRequiredOfflineAllowed`, `Maintenance` and the `Incorrect*` codes the `serverAddress` field is `null`.

Codes: `Ok` | `UpdateRecommended` (address returned) | `UpdateRequired` | `UpdateRequiredOfflineAllowed` | `IncorrectVersion` | `IncorrectServer` | `IncorrectPlatform` | `Maintenance` | `InternalServerError`

---

## CRUD: routing rules

### POST `Prod/add-routing-rule`

**Request:**
```json
{
  "Server": "string",
  "Platform": "string",
  "ClientVersion": "X.Y.Z",
  "RouteTarget": "string",
  "UpdateMode": "UpToDate"
}
```

**Response:**
```json
{ "code": "Ok", "id": "guid", "entry": RoutingConfigurationEntry }
```

**Codes:**
| Code | Reason |
|------|--------|
| `Ok` | Rule created; `id` and `entry` are populated |
| `BadRequest` | Empty fields or invalid `ClientVersion` format |
| `BadConfiguration` | `UpdateMode == None`, or `RouteTarget` / `Server` not found among targets |
| `Conflict` | A rule with the same `Server + Platform + ClientVersion` already exists |
| `InternalServerError` | — |

**Example:**
```bash
curl -X POST http://localhost:5000/Prod/add-routing-rule \
  -H "Authorization: Token" -H "Content-Type: application/json" \
  -d '{"Server":"staging","Platform":"android","ClientVersion":"1.0.0","RouteTarget":"staging","UpdateMode":"UpToDate"}'
```
```json
{"code":"Ok","id":"2ceb3393-3dee-43df-be36-e9dc34dea41b","entry":{...}}
```

---

### POST `Prod/update-routing-rule`
Updates any field of an existing rule by its `id`. The stored `id` does not change.

**Request:**
```json
{
  "Id": "guid",
  "Server": "string",
  "Platform": "string",
  "ClientVersion": "X.Y.Z",
  "RouteTarget": "string",
  "UpdateMode": "UpToDate"
}
```

**Response:**
```json
{ "code": "Ok", "id": "guid", "entry": RoutingConfigurationEntry }
```

**Codes:**
| Code | Reason |
|------|--------|
| `Ok` | Rule updated |
| `BadRequest` | Empty `Id` or invalid fields |
| `NotFound` | No rule with the given `Id` |
| `BadConfiguration` | `UpdateMode == None`, or `RouteTarget` / `Server` not found among targets |
| `InternalServerError` | — |

**Example:**
```bash
curl -X POST http://localhost:5000/Prod/update-routing-rule \
  -H "Authorization: Token" -H "Content-Type: application/json" \
  -d '{"Id":"2ceb3393-...","Server":"staging","Platform":"android","ClientVersion":"1.0.0","RouteTarget":"staging","UpdateMode":"UpdateRecommended"}'
```

---

### POST `Prod/delete-routing-rule`

**Request:**
```json
{ "Id": "guid" }
```

**Response:**
```json
{ "code": "Ok" }
```

**Codes:** `Ok` | `BadRequest` (empty `Id`) | `NotFound` | `InternalServerError`

**Example:**
```bash
curl -X POST http://localhost:5000/Prod/delete-routing-rule \
  -H "Authorization: Token" -H "Content-Type: application/json" \
  -d '{"Id":"2ceb3393-3dee-43df-be36-e9dc34dea41b"}'
```

---

## CRUD: route targets

### POST `Prod/add-route-target`

**Request:**
```json
{ "Target": "string", "Address": "http://...", "Maintenance": false }
```

**Response:**
```json
{ "code": "Ok", "entry": RouteTargetConfigurationEntry }
```

**Codes:**
| Code | Reason |
|------|--------|
| `Ok` | Target created |
| `BadRequest` | Empty fields, or `Address` is not a valid absolute http/https URL |
| `Conflict` | A target with the same name already exists |
| `InternalServerError` | — |

**Example:**
```bash
curl -X POST http://localhost:5000/Prod/add-route-target \
  -H "Authorization: Token" -H "Content-Type: application/json" \
  -d '{"Target":"staging","Address":"http://staging.mock","Maintenance":false}'
```

---

### POST `Prod/update-route-target`
`Id` is the current target name (the storage key). When `Target != Id` the target is renamed: the old record is deleted and a new one is written under the new name.

**Request:**
```json
{ "Id": "current-target-name", "Target": "new-name", "Address": "http://...", "Maintenance": false }
```

**Response:**
```json
{ "code": "Ok", "entry": RouteTargetConfigurationEntry }
```

**Codes:**
| Code | Reason |
|------|--------|
| `Ok` | Target updated |
| `BadRequest` | Empty fields or invalid `Address` |
| `NotFound` | No target with the given `Id` |
| `Conflict` | On rename: a target with name `Target` already exists |
| `InternalServerError` | — |

**Example (rename):**
```bash
curl -X POST http://localhost:5000/Prod/update-route-target \
  -H "Authorization: Token" -H "Content-Type: application/json" \
  -d '{"Id":"staging","Target":"staging-v2","Address":"http://staging-v2.mock","Maintenance":false}'
```

---

### POST `Prod/delete-route-target`
A target cannot be removed while at least one routing rule references it — the call returns `BadRequest` in that case.

**Request:**
```json
{ "Target": "string" }
```

**Response:**
```json
{ "code": "Ok" }
```

**Codes:**
| Code | Reason |
|------|--------|
| `Ok` | Target deleted |
| `BadRequest` | Empty `Target`, or the target is still in use by some rule |
| `NotFound` | Target does not exist |
| `InternalServerError` | — |

---

## Operational endpoints

### POST `Prod/set-maintenance-on` / `POST Prod/set-maintenance-off`
Toggle the `Maintenance` flag on a target.

**Request:**
```json
{ "RouteTarget": "string" }
```

**Response:**
```json
{ "code": "Ok" }
```
Codes: `Ok` | `BadRequest` (empty `RouteTarget`) | `InternalServerError`

---

### POST `Prod/set-routing-configuration`
Bulk replacement of all routing rules. The existing rules are wiped and the new ones are written (each gets a freshly generated GUID).

**Request:**
```json
{ "Configuration": { "Entries": [ RoutingConfigurationEntry, ... ] } }
```
The `Id` field on incoming entries is ignored — DynamoDB assigns a new GUID.

**Response:**
```json
{ "code": "Ok", "configuration": { "entries": [...] } }
```
Codes: `Ok` | `BadRequest` | `BadConfiguration` | `InternalServerError`

---

### POST `Prod/set-route-target-configuration`
Bulk replacement of all targets.

**Request:**
```json
{ "Configuration": { "Entries": [ RouteTargetConfigurationEntry, ... ] } }
```

**Response:**
```json
{ "code": "Ok", "configuration": { "entries": [...] } }
```
Codes: `Ok` | `BadRequest` | `BadConfiguration` | `InternalServerError`

---

### GET `/health`
No authorization. Returns the string `Healthy`.

---

## Validation rules (common)

- `Server` and `RouteTarget` must exist in the route-target configuration as target names.
- `ClientVersion` must follow the `X.Y.Z` format (three integers separated by dots).
- `UpdateMode` cannot be `None`.
- A target's `Address` must be a valid absolute http or https URL (checked by `add-route-target` and `update-route-target`; `set-route-target-configuration` only requires the field to be non-empty).
- A rule's `Id` is a GUID assigned by the server on creation. Use `get-routing-configuration` to fetch the full list of IDs.

## Storage and caching

- **Storage**: DynamoDB (production) / in-memory mock (development; the default in `Startup.cs`).
- **Redis**: hash-based cache. The key `RoutingConfiguration` holds a hash where the field is the rule GUID and the value is the JSON entry. The key `RouteTargetConfiguration` holds a hash where the field is the target name and the value is the JSON entry. CRUD operations issue point-wise `HSET` / `HDEL` without reading the entire list. Bulk `set-*` clears the hash and rewrites it.
- **Proxy**: `RoutingConfigurationStorageProxy2` wires the storage layer and the Redis cache together. The earlier `RoutingConfigurationStorageProxy` is kept around in `Startup.cs` (commented out) as an alternative.