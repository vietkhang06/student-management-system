package com.demo.student_management.repository;

import com.demo.student_management.entity.HocKy;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface HocKyRepository extends JpaRepository<HocKy, String> {
    List<HocKy> findByTenHocKyContainingIgnoreCase(String tenHocKy);
}