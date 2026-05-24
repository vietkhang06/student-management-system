package com.demo.student_management.repository;

import com.demo.student_management.entity.ThamSo;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface ThamSoRepository extends JpaRepository<ThamSo, String> {
    Optional<ThamSo> findByTenThamSo(String tenThamSo);
    List<ThamSo> findByTenThamSoContainingIgnoreCase(String tenThamSo);
}