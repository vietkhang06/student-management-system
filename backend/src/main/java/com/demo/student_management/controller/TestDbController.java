package com.demo.student_management.controller;

import com.demo.student_management.entity.Lop;
import com.demo.student_management.repository.LopRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequiredArgsConstructor
public class TestDbController {

    private final LopRepository lopRepository;

    @GetMapping("/test-db")
    public List<Lop> testDb() {
        return lopRepository.findAll();
    }
}