package com.demo.student_management.repository;

import com.demo.student_management.entity.BanQuanLy;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface BanQuanLyRepository extends JpaRepository<BanQuanLy, String> {
    Optional<BanQuanLy> findByTaiKhoan_IdTaiKhoan(String idTaiKhoan);
}