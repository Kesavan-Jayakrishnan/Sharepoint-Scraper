# Product Use Cases - Document Extraction, Reconciliation, and Report Generation

This document captures practical business use cases for the source-agnostic schema and Jinja-based report generation approach.

## 1) Core Product Purpose

Given one or more business input files (XLSX, CSV, PDF, DOCX, PPTX), the product should:
- extract required values and tables,
- compute derived metrics,
- run reconciliation rules,
- render final DOCX output via Jinja,
- provide a trust-friendly reconciliation summary (Passed/Warning/Failed).

---

## 2) Primary Use Cases

### UC-01: Financial Report from Excel (Deterministic-first)
**Actor:** Finance Analyst  
**Input:** Multi-sheet Excel workbook  
**Output:** FATS DOCX report + reconciliation summary

**Flow:**
1. Upload Excel file.
2. System maps fields and tables from configured sheet/cell/range mappings.
3. Derivations and reconciliation rules execute.
4. Jinja template is populated and DOCX generated.
5. Reconciliation appendix shows ✅ pass / ⚠️ warning / ❌ fail.

---

### UC-02: Financial Report from PDF/DOCX (AI fallback)
**Actor:** Operations Analyst  
**Input:** PDF or DOCX financial pack  
**Output:** FATS DOCX report + reconciliation summary

**Flow:**
1. Upload PDF/DOCX.
2. Document Intelligence converts content to structured HTML/text blocks.
3. Deterministic extraction runs where possible.
4. LLM fallback extracts missing fields/tables.
5. Confidence and reconciliation rules decide publish/block.

---

### UC-03: Mixed Inputs (Excel + PDF notes)
**Actor:** Controller  
**Input:** Excel base + PDF assumptions note  
**Output:** Consolidated report

**Flow:**
1. System extracts structured numbers from Excel.
2. System extracts narrative/risk fields from PDF using AI fallback.
3. Merge and conflict resolution policy applies.
4. Final report and reconciliation are generated.

---

### UC-04: Table-heavy Scenario
**Actor:** Finance Ops  
**Input:** Excel/PDF with large line-item table  
**Output:** Rendered table in DOCX

**Flow:**
1. Extract line-items table with required columns.
2. Validate row-level constraints (non-negative, amount consistency).
3. Aggregate and reconcile against total revenue.
4. Render table via Jinja loop.

---

### UC-05: Business Trust Validation
**Actor:** Business Reviewer  
**Input:** Generated report package  
**Output:** Go/No-Go decision

**Flow:**
1. Reviewer opens report.
2. Checks reconciliation summary counts and rule-level outcomes.
3. If failed rules exist, report is blocked.
4. If warnings only, publish decision follows policy.

---

## 3) Reconciliation Outcome Model

- **PASSED**: All rules pass.
- **PASSED_WITH_WARNING**: No errors, one or more warnings.
- **FAILED**: One or more error-severity rules fail.

This status should be shown both in machine output (`meta.publish_decision`) and human-readable appendix.

---

## 4) Edge Cases Covered

- Missing optional field (kept as null with warning)
- Duplicate table keys (warning)
- Arithmetic mismatch with tolerance
- Date ordering errors
- Empty required table
- Low-confidence AI extraction routed to review
- Conflicting values across sources resolved by policy

---

## 5) Non-Functional Expectations

- Explainability: every extracted value carries lineage metadata.
- Auditability: rule results are persisted for each run.
- Robustness: deterministic-first, AI fallback with retry + review.
- Extensibility: add new report schemas without changing core engine.

---

## 6) Suggested Folder Convention

- `fats_report_schema_v2.yaml` -> business-authoring schema
- `fats_report_schema_v2.json` -> runtime canonical schema
- `jinja_output_contract.json` -> renderer input contract
- `jinja_output_sample_edge_cases.json` -> example output
- `USECASES.md` -> business/functional reference

