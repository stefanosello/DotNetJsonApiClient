---
title: Home
description: Documentation for the DotNetJsonApiClient library.
---

![logo.svg](https://raw.githubusercontent.com/stefanosello/DotNetJsonApiClient/refs/heads/main/logo.svg)

[![codecov](https://codecov.io/gh/stefanosello/DotNetJsonApiClient/graph/badge.svg?token=VGZAWJNI5X)](https://codecov.io/gh/stefanosello/DotNetJsonApiClient)
[![PUBLISH](https://github.com/stefanosello/DotNetJsonApiClient/actions/workflows/publish.yml/badge.svg)](https://github.com/stefanosello/DotNetJsonApiClient/actions/workflows/publish.yml)
[![DOCS](https://github.com/stefanosello/DotNetJsonApiClient/actions/workflows/docs.yml/badge.svg)](https://github.com/stefanosello/DotNetJsonApiClient/actions/workflows/docs.yml)
[![NuGet Version](https://img.shields.io/nuget/v/DotNetJsonApiClient)](https://www.nuget.org/packages/DotNetJsonApiClient#versions-body-tab)
[![Open Source? Yes!](https://badgen.net/badge/Open%20Source%20%3F/Yes%21/blue?icon=github)](https://github.com/Naereen/badges/)


## Purpose
This project is a lightweight `.NET 8` client library designed for APIs adhering to the `json:api` standard. It offers an EF Core-like experience for querying data from `json:api` endpoints, simplifying integration and development workflows.

The library was born out of the need for an easy-to-use solution to fetch data from `HTTP` endpoints in a microservice architecture, where some services act as data providers and others as consumers. Its intuitive design makes it an excellent companion to the [JsonApiDotNetCore](https://www.jsonapi.net/index.html) library, as both adhere to the same standard, ensuring seamless interoperability.
