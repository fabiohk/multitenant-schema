#!/usr/bin/env bash
set -ex

PROJECT=MultiTenantSchema
export DB_SCHEMA=$1

echo "Applying migrations..."
dotnet ef database update --project ${PROJECT} --verbose