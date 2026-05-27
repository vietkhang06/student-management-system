package com.demo.student_management.controller;

import com.demo.student_management.dto.hocsinh.HocSinhCreateRequest;
import com.demo.student_management.dto.hocsinh.HocSinhResponse;
import com.demo.student_management.dto.hocsinh.HocSinhSummaryResponse;
import com.demo.student_management.dto.hocsinh.HocSinhUpdateRequest;
import com.demo.student_management.security.AuthorizationService;
import com.demo.student_management.service.HocSinhService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/hoc-sinh")
@RequiredArgsConstructor
public class HocSinhController {

    private final HocSinhService hocSinhService;
    private final AuthorizationService authorizationService;

    @PostMapping
    public HocSinhResponse create(@RequestBody HocSinhCreateRequest request) {
        authorizationService.requireAdmin();
        return hocSinhService.create(request);
    }

    @GetMapping
    public List<HocSinhResponse> getAll() {
        List<HocSinhResponse> all = hocSinhService.getAll();
        if (authorizationService.isBanQuanLy()) {
            return all;
        }
        java.util.Set<String> assignedClassIds = authorizationService.getAssignedClassIdsForCurrentTeacher();
        return all.stream()
                .filter(hs -> assignedClassIds.contains(hs.getIdLop()))
                .toList();
    }

    @GetMapping("/{idHocSinh}")
    public HocSinhResponse getById(@PathVariable String idHocSinh) {
        HocSinhResponse hs = hocSinhService.getById(idHocSinh);
        if (!authorizationService.isBanQuanLy()) {
            authorizationService.requireCanAccessClass(hs.getIdLop());
        }
        return hs;
    }

    @GetMapping("/search")
    public List<HocSinhResponse> search(@RequestParam String ten) {
        List<HocSinhResponse> searchResults = hocSinhService.searchByName(ten);
        if (authorizationService.isBanQuanLy()) {
            return searchResults;
        }
        java.util.Set<String> assignedClassIds = authorizationService.getAssignedClassIdsForCurrentTeacher();
        return searchResults.stream()
                .filter(hs -> assignedClassIds.contains(hs.getIdLop()))
                .toList();
    }

    @GetMapping("/summary")
    public List<HocSinhSummaryResponse> getStudentSummaries() {
        List<HocSinhSummaryResponse> summaries = hocSinhService.getStudentSummaries();
        if (authorizationService.isBanQuanLy()) {
            return summaries;
        }
        java.util.Set<String> assignedClassIds = authorizationService.getAssignedClassIdsForCurrentTeacher();
        return summaries.stream()
                .filter(hs -> assignedClassIds.contains(hs.getIdLop()))
                .toList();
    }

    @PutMapping("/{idHocSinh}")
    public HocSinhResponse update(@PathVariable String idHocSinh, @RequestBody HocSinhUpdateRequest request) {
        authorizationService.requireAdmin();
        return hocSinhService.update(idHocSinh, request);
    }

    @DeleteMapping("/{idHocSinh}")
    public ResponseEntity<Void> delete(@PathVariable String idHocSinh) {
        authorizationService.requireAdmin();
        hocSinhService.delete(idHocSinh);
        return ResponseEntity.ok().build();
    }

    @GetMapping("/{idHocSinh}/has-scores")
    public ResponseEntity<Boolean> hasScores(@PathVariable String idHocSinh) {
        if (!authorizationService.isBanQuanLy()) {
            HocSinhResponse hs = hocSinhService.getById(idHocSinh);
            authorizationService.requireCanAccessClass(hs.getIdLop());
        }
        return ResponseEntity.ok(hocSinhService.hasScores(idHocSinh));
    }
}
