{{- define "telemetry-intake-api.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end }}

{{- define "telemetry-intake-api.fullname" -}}
telemetry-intake-api
{{- end }}

{{- define "telemetry-intake-user-api.fullname" -}}
telemetry-intake-user-api
{{- end }}

{{- define "telemetry-intake-api.labels" -}}
helm.sh/chart: {{ .Chart.Name }}-{{ .Chart.Version | replace "+" "_" }}
app.kubernetes.io/name: {{ include "telemetry-intake-api.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/version: {{ .Chart.AppVersion }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{- define "telemetry-intake-api.selectorLabels" -}}
app.kubernetes.io/name: {{ include "telemetry-intake-api.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{- define "telemetry-intake-api.serviceAccountName" -}}
{{- if .Values.serviceAccount.name -}}
{{- .Values.serviceAccount.name -}}
{{- else -}}
{{- include "telemetry-intake-api.fullname" . -}}
{{- end -}}
{{- end }}