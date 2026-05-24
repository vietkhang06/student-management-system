package com.demo.student_management.repository;

import com.demo.student_management.entity.GiaoVien;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface GiaoVienRepository extends JpaRepository<GiaoVien, String> {
    Optional<GiaoVien> findByTaiKhoan_IdTaiKhoan(String idTaiKhoan);
}