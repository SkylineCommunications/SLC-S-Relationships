# SLC-S-Relationships

This repository contains the source code for the **Relationships** solution — a DataMiner standard solution that provides the tools to establish, store, and query relationships between objects in a DataMiner System. It includes a deployable package, GQI data sources for querying the relationship graph, and an SDM registration script.

> [!TIP]
> The outcome of this repository is available in the Catalog: [Relationships | Catalog | dataminer.services](https://catalog.dataminer.services/details/06e517f4-5041-41a1-a186-86cdbf0410b9)

## Projects

| Project | Description | Type |
|---------|-------------|------|
| [Relationships.GQI](Relationships.GQI/README.md) | Three GQI ad-hoc data sources for querying entity nodes, relationship edges, and searching entities in the Relationships data model. | Ad-Hoc Data Source |
| [Relationships.Package](Relationships.Package/README.md) | Deployable DataMiner package that bundles all Relationships solution components for installation on a DataMiner System. | Package |
| [Relationships.Package.Registration](Relationships.Package.Registration/README.md) | Internal automation script that registers the Relationships solution and its data models with the DataMiner SDM registry during package installation. | Automation Script |
