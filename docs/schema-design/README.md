# Document Extraction Schema & Jinja Output Contract

This folder contains production-ready artifacts for a source-agnostic document extraction pipeline:

1. **`fats_report_schema_v2.yaml`** - business-friendly schema (authoring format).
2. **`fats_report_schema_v2.json`** - canonical runtime JSON form.
3. **`jinja_output_contract.json`** - output structure required by Jinja rendering.
4. **`jinja_output_sample_edge_cases.json`** - full sample output covering edge cases.

## Intent

These files are designed for a pipeline where binary inputs (e.g., XLSX/PDF/DOCX/PPTX/CSV) are normalized via Document Intelligence, extracted using deterministic selectors with LLM fallback, reconciled using business-owned rules, and rendered into DOCX using Jinja.

## How to use

- Business users can edit the YAML schema.
- Engineering can transform YAML -> C# POCO -> JSON for runtime.
- Renderer consumes `jinja_output_contract.json` shape and data values from runtime output.
