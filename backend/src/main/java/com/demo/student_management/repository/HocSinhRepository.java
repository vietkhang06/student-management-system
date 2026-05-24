package com.demo.student_management.repository;

import com.demo.student_management.entity.HocSinh;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface HocSinhRepository extends JpaRepository<HocSinh, String> {

    List<HocSinh> findByLop_IdLop(String idLop);

    long countByLop_IdLop(String idLop);

    List<HocSinh> findByTenContainingIgnoreCase(String ten);
}