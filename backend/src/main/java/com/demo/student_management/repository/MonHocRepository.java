package com.demo.student_management.repository;

import com.demo.student_management.entity.MonHoc;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface MonHocRepository extends JpaRepository<MonHoc, String> {
    List<MonHoc> findByTenMonHocContainingIgnoreCase(String tenMonHoc);
}