package com.demo.student_management.repository;

import com.demo.student_management.entity.Lop;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface LopRepository extends JpaRepository<Lop, String> {
    List<Lop> findByTenLopContainingIgnoreCase(String tenLop);
}