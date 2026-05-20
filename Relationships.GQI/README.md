# Relationships.GQI

**Project Type**: Ad-Hoc Data Source

**Size**: M

## Summary

**Relationships.GQI** provides three **GQI ad-hoc data sources** that expose the Relationships data model to DataMiner dashboards and low-code apps. These data sources allow users to query **entity nodes** and **relationship edges** starting from a given entity with configurable traversal depth, as well as to **search and filter entities** across the entire Relationships data model. They are the primary integration point between the Relationships solution and the DataMiner GQI query engine.

## Relationships.GQI.Entity.Nodes

Retrieves entity **nodes** from the Relationships data model by traversing the relationship graph starting from a given entity up to a specified depth. The data source returns a flat list of related entities, each with metadata such as display name, model name, solution, status, and parent reference. It is designed to be paired with the `Relationships.GQI.Entity.Edges` data source to build a graph or topology visualization in low-code apps or dashboards.

**Interfaces**: `IGQIDataSource`, `IGQIOnInit`, `IGQIInputArguments`, `IGQIOnPrepareFetch`, `IGQIOnDestroy`

### Input Arguments

| Name | Type | Required | Description |
|------|------|----------|-------------|
| Entity ID | String | Yes | The unique identifier of the starting entity. |
| Entity Model Name | String | Yes | The model name of the starting entity. |
| Entity Solution ID | String | Yes | The solution ID of the starting entity. |
| Depth | Int | No | The traversal depth (min: 1, max: 16). Defaults to 1. |
| Include InActive Relations | Boolean | No | Whether to include inactive entities in the result. Defaults to false. |

### Output Columns

| Name | Type | Description |
|------|------|-------------|
| Key | String | The composite key of the entity (SolutionID/ModelName/ID). |
| ID | String | The unique identifier of the entity. |
| Status | Int | The entity status (0 = Active, 1 = Inactive). |
| Display Name | String | The human-readable name of the entity. |
| Model Name | String | The model the entity belongs to. |
| Solution ID | String | The ID of the solution that owns this entity. |
| Solution Name | String | The display name of the solution that owns this entity. |
| Parent ID | String | The ID of the parent entity, if any. |
| Parent Model Name | String | The model name of the parent entity, if any. |

## Relationships.GQI.Entity.Edges

Retrieves **relationship edges** between entities by traversing the relationship graph starting from a given entity up to a specified depth. The data source returns all directed relations encountered during the traversal, identified by source and target entity keys. It is designed to be paired with `Relationships.GQI.Entity.Nodes` to render graph or topology visualizations in low-code apps or dashboards.

**Interfaces**: `IGQIDataSource`, `IGQIOnInit`, `IGQIInputArguments`, `IGQIOnPrepareFetch`, `IGQIOnDestroy`

### Input Arguments

| Name | Type | Required | Description |
|------|------|----------|-------------|
| Entity ID | String | Yes | The unique identifier of the starting entity. |
| Entity Model Name | String | Yes | The model name of the starting entity. |
| Entity Solution ID | String | Yes | The solution ID of the starting entity. |
| Depth | Int | No | The traversal depth (min: 1, max: 16). Defaults to 1. |
| Include InActive Relations | Boolean | No | Whether to include relations involving inactive entities. Defaults to false. |

### Output Columns

| Name | Type | Description |
|------|------|-------------|
| From ID | String | The entity key of the source entity. |
| To ID | String | The entity key of the target entity. |
| Status | Int | The edge status (0 = Active, 1 = Inactive). |

## Relationships.GQI.SearchEntity

Provides a **searchable and filterable list of entity descriptors** from the Relationships data model. All filter arguments are optional — when provided, they are applied as substring filters using AND logic. Results are sorted by relevance (best match score) and then alphabetically by ID, making this data source suitable for use in **dropdown components** and search-driven UI elements in low-code apps.

**Interfaces**: `IGQIDataSource`, `IGQIOnInit`, `IGQIInputArguments`, `IGQIOnPrepareFetch`, `IGQIOnDestroy`

### Input Arguments

| Name | Type | Required | Description |
|------|------|----------|-------------|
| ID | String | No | Filter by entity ID (substring match). |
| Display Name | String | No | Filter by entity display name (substring match). |
| Model Name | String | No | Filter by model name (substring match). |
| Solution ID | String | No | Filter by solution ID (substring match). |
| Solution Name | String | No | Filter by solution name (substring match). |
| Parent ID | String | No | Filter by parent entity ID (substring match). |
| Parent Model Name | String | No | Filter by parent model name (substring match). |

### Output Columns

| Name | Type | Description |
|------|------|-------------|
| ID | String | The unique identifier of the entity. |
| Status | Int | The entity status (0 = Active, 1 = Inactive). |
| Display Name | String | The human-readable name of the entity. |
| Model Name | String | The model the entity belongs to. |
| Solution ID | String | The ID of the solution that owns this entity. |
| Solution Name | String | The display name of the solution that owns this entity. |
| Parent ID | String | The ID of the parent entity, if any. |
| Parent Model Name | String | The model name of the parent entity, if any. |
| Dropdown | String | Pre-formatted label for dropdowns: "{SolutionName} - {ModelName}: {DisplayName} (ID: {ID})". |
