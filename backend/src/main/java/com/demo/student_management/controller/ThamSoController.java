package com.demo.student_management.controller;

import com.demo.student_management.dto.thamso.ThamSoResponse;
import com.demo.student_management.dto.thamso.ThamSoUpdateRequest;
import com.demo.student_management.service.ThamSoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;
import java.util.List;

@RestController
@RequestMapping("/api/tham-so")
@RequiredArgsConstructor
public class ThamSoController {

    private final ThamSoService thamSoService;

    @GetMapping
    public List<ThamSoResponse> getAll() {
        return thamSoService.getAll();
    }

    @GetMapping("/{idThamSo}")
    public ThamSoResponse getById(@PathVariable String idThamSo) {
        return thamSoService.getById(idThamSo);
    }

    @PutMapping("/{idThamSo}")
    public ThamSoResponse update(@PathVariable String idThamSo, @RequestBody ThamSoUpdateRequest request) {
        return thamSoService.update(idThamSo, request);
    }
}
