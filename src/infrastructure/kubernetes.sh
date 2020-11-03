#!/bin/bash

# Following two should be passed as environment variable
# clientId=$1
# clientSecret=$2
# subscriptionId=$3
# aadTenantId=$4
# resourceGroup=$5
# location=$6
# clusterName=$7

echo "Selecting subscription.."
az account set --subscription $subscriptionId

echo "Creating (if not exists) resource group: $resourceGroup."
echo "Data center Location: $location."

group=$(az group create --name $resourceGroup \
    --location $location \
    --tags Purpose=Demo Production=NO \
    --managed-by "moim.hossain@microsoft.com")

az configure --defaults group=$resourceGroup

echo "Creating the cluster..."

# Creating the cluster - wtih AAD
az aks create \
    -g $resourceGroup \
    -n $clusterName \
    --service-principal $clientId \
    --client-secret $clientSecret \
    --location $location \
    --generate-ssh-keys \
    --node-count 3 \
    --network-plugin azure \
    --network-policy azure \
    --enable-cluster-autoscaler \
    --min-count 1 \
    --max-count 3    

echo "Cluster created successfully."