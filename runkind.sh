docker build -t currency-converter:1.0 .
kind create cluster --config=kind.yaml
kind load docker-image currency-converter.yaml
kubectl apply -f ./kubernetes/

watch -n1 kubectl get deployments,pods,services