# Relationships.Package.Registration

**Project Type**: Automation Script

**Size**: XS

## Summary

**Relationships.Package.Registration** is an internal **SDM registration** script that runs during package installation. It registers the Relationships solution and its two core data models — **EntityDescriptor** and **Relation** — with the DataMiner **Standard Data Model (SDM)** registry. Before registering, it cleans up any previous registration to ensure a consistent state.

## Purpose

This is an internal helper script called automatically during package installation. It is not intended to be triggered manually by operators.

## Interactions

- **SDM Registry**: Creates or updates a `SolutionRegistration` for the Relationships solution and `ModelRegistration` objects for the `EntityDescriptor` and `Relation` models.

## Input Parameters

| Name | Description | Format |
|------|-------------|--------|
| version | The version of the package being installed. | string |
